using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using TS.UI.Controls;

namespace TS.Library
{
    public class clsVehicle : INotifyPropertyChanged
    {
        #region Public Properties
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler JourneyCompleted, StartedMoving, ChunkChanged;
        public int VehicleID { get; set; }
        public double MaxSpeed { get; set; }
        public bool Reckless { get; set; }
        public enumVehicleType VehicleType { get; set; }
        public clsChunkCollection ChunkCollection { get; set; }
        public clsRoute Route { get; set; }
        public bool IsTrafficLightAhead { get; set; }
        public bool IsStopped { get; set; }
        public int CurrentPosition { get; set; }
        public ctrlVehicle VehicleVisual { get; set; }
        public Storyboard MoveStoryboard { get; set; }
        public double LengthToZeroSpeed, SpeedUntilDecelerate;
        #endregion

        #region Fields
        double m_dblCurrentSpeed;
        public double CurrentSpeed
        {
            get
            {
                return m_dblCurrentSpeed;
            }
            set
            {
                m_dblCurrentSpeed = value;
                OnPropertyChanged("CurrentSpeed");
            }
        }

        double m_dblAverageSpeed;
        public double AverageSpeed
        {
            get
            {
                return m_dblAverageSpeed;
            }
            set
            {
                m_dblAverageSpeed = value;
                OnPropertyChanged("AverageSpeed");
            }
        }

        double m_dblChunkCompleted;
        public double ChunkCompleted
        {
            get
            {
                return m_dblChunkCompleted;
            }
            set
            {
                m_dblChunkCompleted = value;
                OnPropertyChanged("ChunkCompleted");
            }
        }

        TimeSpan m_objCompleteTime;
        public TimeSpan CompleteTime
        {
            get
            {
                return m_objCompleteTime;
            }
            set
            {
                m_objCompleteTime = value;
                OnPropertyChanged("CompleteTime");
            }
        }

        bool m_isAnimationShown;
        public bool ShowAnimation
        {
            get { return m_isAnimationShown; }
            set
            {
                m_isAnimationShown = value;
                if (!m_isAnimationShown)
                    VehicleVisual.Visibility = Visibility.Hidden;
                else
                    VehicleVisual.Visibility = Visibility.Visible;
            }
        }
        #endregion

        #region Private Properties
        bool m_isAccelerate;
        bool m_isFirstStart = true;
        long m_longStartTime;
        TimeSpan m_objCurrentStartTime, m_objRemainingTime;
        int m_intAverageTicks = 0;
        Random m_objRandom;
        DispatcherTimer m_objNextTimer, m_objAverageSpeedTimer;
        clsIntersection m_objWaitAtIntersection;
        #endregion

        public clsVehicle()
        {
            CurrentPosition = 0;
            IsTrafficLightAhead = false;
            ChunkCollection = new clsChunkCollection();
            Route = new clsRoute();
            m_objRandom = new Random();

            m_objNextTimer = new DispatcherTimer();
            m_objNextTimer.Tick += m_objNextTimer_Tick;

            m_objAverageSpeedTimer = new DispatcherTimer();
            m_objAverageSpeedTimer.Interval = TimeSpan.FromMilliseconds(1000 / clsConfig.SuperTime);
            m_objAverageSpeedTimer.Tick += new EventHandler(m_objAverageSpeedTimer_Tick);
        }

        #region Public Functions
        public void StartMoving()
        {
            m_objCurrentStartTime = TimeSpan.FromTicks(DateTime.Now.Ticks);

            if (m_isFirstStart)
            {
                m_longStartTime = DateTime.Now.Ticks;
                m_objAverageSpeedTimer.Start();
                m_isFirstStart = false;
            }

            LengthToZeroSpeed = 0;

            if (CurrentPosition < ChunkCollection.Count)
            {
                Point _objDesPoint = new Point(Route[CurrentPosition].X2, Route[CurrentPosition].Y2);
                if (clsGlobalStorage.AllIntersections.AllTLPoints.Contains(_objDesPoint))
                {
                    m_objWaitAtIntersection = clsGlobalStorage.AllIntersections.GetIntersection(_objDesPoint);
                    IsTrafficLightAhead = true;
                }

                if (ChunkCollection[CurrentPosition].GetVehicleCount() < ChunkCollection[CurrentPosition].MaxVehicleCount)
                {
                    ChunkCollection[CurrentPosition].InsertVehicle(this);
                    CalculateSpeed();

                    double _dblSpeed = ChunkCollection[CurrentPosition].RealLength / CurrentSpeed * 1000;
                    m_objNextTimer.Interval = TimeSpan.FromMilliseconds(_dblSpeed / clsConfig.SuperTime);
                    m_objNextTimer.Start();

                    if (ChunkChanged != null)
                        ChunkChanged(this, new EventArgs());
                    if (StartedMoving != null)
                        StartedMoving(this, new EventArgs());
                }
                else
                {
                    CurrentSpeed = 0;
                }
            }
            else
            {
                CompleteJourney();
            }
        }

        public void CalculateSpeed()
        {
            if (!Reckless)
            {
                if (ChunkCollection[CurrentPosition].SpeedLimit < MaxSpeed)
                    CurrentSpeed = ChunkCollection[CurrentPosition].SpeedLimit - 5 + m_objRandom.Next(0, 6);
                else
                    CurrentSpeed = MaxSpeed - 5 + m_objRandom.Next(0, 6);
            }
            else
            {
                if (ChunkCollection[CurrentPosition].SpeedLimit < MaxSpeed)
                    CurrentSpeed = ChunkCollection[CurrentPosition].SpeedLimit + m_objRandom.Next(1, 11);
                else
                    CurrentSpeed = MaxSpeed + m_objRandom.Next(1, 11);
            }

            if (ChunkCollection[CurrentPosition].RecklessCount > 0)
            {
                CurrentSpeed = CurrentSpeed * (1 - ChunkCollection[CurrentPosition].RecklessCount * clsConfig.RecklessEffect);
                if (!clsGlobalStorage.SlowedVehicles.Contains(this))
                    clsGlobalStorage.SlowedVehicles.Add(this);
            }

            if (ChunkCollection[CurrentPosition].VehicleCollection.Count > ChunkCollection[CurrentPosition].NonCongestedCount)
            {
                int _intExtraVehicle = ChunkCollection[CurrentPosition].VehicleCollection.Count - ChunkCollection[CurrentPosition].NonCongestedCount;
                int _intTillMax = ChunkCollection[CurrentPosition].MaxVehicleCount - ChunkCollection[CurrentPosition].NonCongestedCount;
                double _dblSlowest, _dblDimishedSpeed = 0;
                if (!Reckless)
                    _dblSlowest = CurrentSpeed / 2;
                else
                    _dblSlowest = CurrentSpeed / 1.5;
                _dblDimishedSpeed = CurrentSpeed - _dblSlowest;
                double _dblRate = (double)_intExtraVehicle / (double)_intTillMax;

                CurrentSpeed = _dblSlowest + _dblDimishedSpeed * (1 - _dblRate);
            }

            if (IsTrafficLightAhead || m_isAccelerate)
            {
                LengthToZeroSpeed = CurrentSpeed / 10 * clsConfig.AccelerationLength;
                double _dblAccelerationRatio = LengthToZeroSpeed / ChunkCollection[CurrentPosition].RealLength;
                CurrentSpeed = CurrentSpeed * (1 - _dblAccelerationRatio) + (CurrentSpeed / 2) * _dblAccelerationRatio;
                m_isAccelerate = false;
            }
        }

        public Point GetCurrentPosition()
        {
            Point _objCurrentPoint = new Point();

            if (!IsStopped && CurrentSpeed > 0)
            {
                TimeSpan _objSupposedTime = m_objCurrentStartTime + m_objNextTimer.Interval;
                TimeSpan _objElapsedTime = _objSupposedTime - TimeSpan.FromTicks(DateTime.Now.Ticks);
                m_objRemainingTime = m_objNextTimer.Interval - _objElapsedTime;
                if (m_objRemainingTime.TotalMilliseconds <= 0)
                    m_objRemainingTime = TimeSpan.FromMilliseconds(1);

                double _dblCompletion = 100 - ((double)_objElapsedTime.Ticks / (double)m_objNextTimer.Interval.Ticks) * 100;
                if (_dblCompletion < 0) _dblCompletion = 0;
                ChunkCompleted = _dblCompletion;

                clsPointSet _objRoute = Route[CurrentPosition];
                double _dblDistX = _objRoute.X1 - _objRoute.X2;
                double _dblDistY = _objRoute.Y1 - _objRoute.Y2;

                if (_dblDistX < 0)
                    _objCurrentPoint.X = _objRoute.X1 + (Math.Abs(_dblDistX) * ChunkCompleted / 100);
                else
                    _objCurrentPoint.X = _objRoute.X1 - (_dblDistX * ChunkCompleted / 100);
                if (_dblDistY < 0)
                    _objCurrentPoint.Y = _objRoute.Y1 + (Math.Abs(_dblDistY) * ChunkCompleted / 100);
                else
                    _objCurrentPoint.Y = _objRoute.Y1 - (_dblDistY * ChunkCompleted / 100);
            }
            else
            {
                if (IsStopped)
                {
                    _objCurrentPoint.X = Route[CurrentPosition].X2;
                    _objCurrentPoint.Y = Route[CurrentPosition].Y2;
                }
                else
                {
                    _objCurrentPoint.X = Route[CurrentPosition - 1].X2;
                    _objCurrentPoint.Y = Route[CurrentPosition - 1].Y2;
                }
            }

            return _objCurrentPoint;
        }
        #endregion

        #region Private Functions
        private bool CheckIfNextFull()
        {
            int _intNextPosition = CurrentPosition + 1;
            if (_intNextPosition < ChunkCollection.Count)
                if (ChunkCollection[_intNextPosition].GetVehicleCount() < ChunkCollection[_intNextPosition].MaxVehicleCount)
                    return false;
            return true;
        }

        private void CompleteJourney()
        {
            m_objAverageSpeedTimer.Stop();
            m_objAverageSpeedTimer.Tick -= m_objAverageSpeedTimer_Tick;
            m_objAverageSpeedTimer = null;
            AverageSpeed = AverageSpeed / m_intAverageTicks;
            m_objNextTimer.Tick -= m_objNextTimer_Tick;
            m_objNextTimer = null;
            CurrentSpeed = 0;
            CompleteTime = TimeSpan.FromTicks((DateTime.Now.Ticks - m_longStartTime) * clsConfig.SuperTime);
            clsGlobalStorage.CompletedVehicles.Add(this);
            clsGlobalStorage.RemoveVehicle(this);
            if (JourneyCompleted != null)
                JourneyCompleted(this, new EventArgs());
        }
        #endregion

        #region Event Handling
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void m_objAverageSpeedTimer_Tick(object sender, EventArgs e)
        {
            AverageSpeed += CurrentSpeed;
            m_intAverageTicks++;

            clsGlobalStorage.TotalEvents++;
        }

        private void m_objNextTimer_Tick(object sender, EventArgs e)
        {
            m_objNextTimer.Stop();

            if (IsTrafficLightAhead)
            {
                if (VehicleID == 14)
                {
                }
                IsStopped = true;
                IsTrafficLightAhead = false;
            }

            //if (!IsStopped || (IsStopped && m_objWaitAtIntersection.AdjacentChunks[m_objWaitAtIntersection.CurrentDirection] == ChunkCollection[CurrentPosition]))
            if (!IsStopped)
            {
                if (!CheckIfNextFull())
                {
                    IsStopped = false;
                    IsTrafficLightAhead = false;
                    ChunkCollection[CurrentPosition].RemoveVehicle(this);
                    CurrentPosition++;
                    StartMoving();
                }
                else
                {
                    m_isAccelerate = true;
                    CurrentSpeed = 0;
                    CurrentPosition++;
                    if (CurrentPosition < ChunkCollection.Count)
                        ChunkCollection[CurrentPosition].VehicleRemoved += new EventHandler(clsVehicle_VehicleRemoved);
                    else
                    {
                        ChunkCollection[--CurrentPosition].RemoveVehicle(this);
                        CompleteJourney();
                    }
                }
            }
            else
            {
                CurrentSpeed = 0;
                IsStopped = true;
            }

            clsGlobalStorage.TotalEvents++;
        }

        private void clsVehicle_VehicleRemoved(object sender, EventArgs e)
        {
            if (ChunkCollection[CurrentPosition].GetVehicleCount() < ChunkCollection[CurrentPosition].MaxVehicleCount)
            {
                ChunkCollection[CurrentPosition].VehicleRemoved -= clsVehicle_VehicleRemoved;
                ChunkCollection[CurrentPosition - 1].RemoveVehicle(this);
                //IsStopped = true;
                StartMoving();
            }

            clsGlobalStorage.TotalEvents++;
        }
        #endregion

        #region Commented Storyboard Code
        //public void Init()
        //{
        //    if (VehicleVisual != null)
        //    {
        //        TransformGroup _objTransGroup = new TransformGroup();
        //        _objTransGroup.Children.Add(new ScaleTransform());
        //        _objTransGroup.Children.Add(new SkewTransform());
        //        _objTransGroup.Children.Add(new RotateTransform());
        //        _objTransGroup.Children.Add(new TranslateTransform());
        //        VehicleVisual.RenderTransform = _objTransGroup;

        //        m_objTransformGroup = (TransformGroup)VehicleVisual.RenderTransform;
        //        m_objTranslateTransform = (TranslateTransform)m_objTransformGroup.Children[3];

        //        MoveStoryboard = new Storyboard();
        //        MoveStoryboard.Name = "Move" + VehicleID;
        //        MoveStoryboard.Completed += MoveStoryboard_Completed;

        //        VehicleVisual.Name = "VG" + VehicleID;
        //        VehicleVisual.Width = 16;
        //        VehicleVisual.Height = 16;
        //    }
        //}

        //public void MoveNext()
        //{
        //    if (CurrentPosition < Route.Count)
        //    {
        //        ChunkCollection[CurrentPosition].InsertVehicle(this);

        //        long _dblSpeed = (long)CalculateSpeed();
                
        //        DoubleAnimationUsingKeyFrames _objAnim = new DoubleAnimationUsingKeyFrames();
        //        EasingDoubleKeyFrame _objKeyFrame = new EasingDoubleKeyFrame();
        //        _objKeyFrame.Value = Route[CurrentPosition].SrcX - clsConfig.AdjustX;
        //        _objKeyFrame.KeyTime = TimeSpan.FromSeconds(0);
        //        _objAnim.KeyFrames.Add(_objKeyFrame);

        //        _objKeyFrame = new EasingDoubleKeyFrame();
        //        _objKeyFrame.Value = Route[CurrentPosition].DesX - clsConfig.AdjustX;
        //        _objKeyFrame.KeyTime = TimeSpan.FromTicks(_dblSpeed);
        //        _objAnim.KeyFrames.Add(_objKeyFrame);

        //        Storyboard.SetTarget(_objAnim, VehicleVisual);
        //        Storyboard.SetTargetProperty(_objAnim, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.X)"));
        //        MoveStoryboard.Children.Add(_objAnim);

        //        _objAnim = new DoubleAnimationUsingKeyFrames();

        //        _objKeyFrame = new EasingDoubleKeyFrame();
        //        _objKeyFrame.Value = Route[CurrentPosition].SrcY - clsConfig.AdjustY;
        //        _objKeyFrame.KeyTime = TimeSpan.FromSeconds(0);
        //        _objAnim.KeyFrames.Add(_objKeyFrame);

        //        _objKeyFrame = new EasingDoubleKeyFrame();
        //        _objKeyFrame.Value = Route[CurrentPosition].DesY - clsConfig.AdjustY;
        //        _objKeyFrame.KeyTime = TimeSpan.FromTicks(_dblSpeed);
        //        _objAnim.KeyFrames.Add(_objKeyFrame);

        //        Storyboard.SetTarget(_objAnim, VehicleVisual);
        //        Storyboard.SetTargetProperty(_objAnim, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.Y)"));
        //        MoveStoryboard.Children.Add(_objAnim);

        //        MoveStoryboard.Begin();
        //    }
        //}

        //private void MoveStoryboard_Completed(object sender, EventArgs e)
        //{
        //    if (CurrentPosition < Route.Count)
        //    {
        //        ChunkCollection[CurrentPosition].RemoveVehicle(this);
        //        CurrentPosition++;
        //        MoveNext();
        //    }
        //}
        #endregion

        #region Retired Pausing Functions
        public void PauseTimer()
        {
            m_objNextTimer.Stop();

            TimeSpan _objSupposedTime = m_objCurrentStartTime + m_objNextTimer.Interval;
            TimeSpan _objElapsedTime = _objSupposedTime - TimeSpan.FromTicks(DateTime.Now.Ticks);
            m_objRemainingTime = m_objNextTimer.Interval - _objElapsedTime;

            if (m_objRemainingTime.TotalMilliseconds <= 0)
                m_objRemainingTime = TimeSpan.FromMilliseconds(1);

            double _dblCompletion = ((double)_objElapsedTime.Ticks / (double)m_objNextTimer.Interval.Ticks) * 100;
            if (_dblCompletion < 0) _dblCompletion = 0;
            ChunkCompleted = _dblCompletion;
        }

        public void ResumeTimer()
        {
            m_objNextTimer.Interval = m_objRemainingTime;
            m_objNextTimer.Start();
            ChunkCompleted = 0;
        }

        public bool ResumeMoving()
        {
            if (CurrentPosition + 1 >= ChunkCollection.Count)
                return false;

            if (ChunkCollection[CurrentPosition + 1].GetVehicleCount() < ChunkCollection[CurrentPosition + 1].MaxVehicleCount)
            {
                IsStopped = false;
                m_isAccelerate = true;
                ChunkCollection[CurrentPosition].RemoveVehicle(this);
                CurrentPosition++;
                StartMoving();
            }

            return true;
        }
        #endregion
    }
}
