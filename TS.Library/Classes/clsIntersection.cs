using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using TS.UI.Controls;

namespace TS.Library
{
    public class clsIntersection
    {
        public int IntersectionID { get; set; }
        public clsChunkCollection AdjacentChunks { get; set; }
        public ctrlTrafficLight TrafficLightVisual { get; set; }
        public DispatcherTimer ChangeTimer { get; set; }
        public int CurrentDirection { get; set; }
        public Point TrafficLightPosition { get; set; }

        DispatcherTimer m_objAccelerateTimer, m_objCountDownTimer;
        TimeSpan m_objStartTime, m_objPauseTime;
        int m_intCurrentSecond;
        bool m_isTrafficLight = false;

        #region Fields
        public bool IsTrafficLight
        {
            get
            {
                return m_isTrafficLight;
            }
            set
            {
                m_isTrafficLight = value;
                if (m_isTrafficLight)
                    InitTrafficLight();
                else
                    RemoveTrafficLight();
            }
        }
        #endregion

        public clsIntersection()
        {
            AdjacentChunks = new clsChunkCollection();
            m_objAccelerateTimer = new DispatcherTimer();
            m_objAccelerateTimer.Interval = TimeSpan.FromMilliseconds(clsConfig.AccelerateTime / clsConfig.SuperTime);
            m_objAccelerateTimer.Tick += new EventHandler(m_objAccelerateTimer_Tick);

            m_objCountDownTimer = new DispatcherTimer();
            m_objCountDownTimer.Interval = TimeSpan.FromMilliseconds(1000 / clsConfig.SuperTime);
            m_objCountDownTimer.Tick += new EventHandler(m_objCountDownTimer_Tick);

            clsGlobalStorage.TotalThreads += 3;
        }

        private void m_objCountDownTimer_Tick(object sender, EventArgs e)
        {
            m_intCurrentSecond--;

            TrafficLightVisual.CountDown = m_intCurrentSecond;

            clsGlobalStorage.TotalThreads++;
        }

        #region Public Functions
        public void InsertAdjacentChunk(int p_intChunkID)
        {
            for (int i = 0; i < clsGlobalStorage.AllChunks.Count; i++)
            {
                if (clsGlobalStorage.AllChunks[i].ChunkID == p_intChunkID)
                {
                    AdjacentChunks.Add(clsGlobalStorage.AllChunks[i]);
                    break;
                }
            }
        }

        public void ReInit()
        {
            ChangeTimer.Stop();
            ChangeTimer.Interval = TimeSpan.FromMilliseconds((clsConfig.TrafficLightTimer * 1000) / clsConfig.SuperTime);
            ChangeTimer.Start();
            m_objCountDownTimer.Stop();
            m_objCountDownTimer.Interval = TimeSpan.FromMilliseconds(1000 / clsConfig.SuperTime);
            m_intCurrentSecond = clsConfig.TrafficLightTimer;
            TrafficLightVisual.CountDown = m_intCurrentSecond;
            m_objCountDownTimer.Start();
            m_objAccelerateTimer.Stop();
            m_objAccelerateTimer.Interval = TimeSpan.FromMilliseconds(clsConfig.AccelerateTime / clsConfig.SuperTime);
            m_objAccelerateTimer.Start();
        }

        public void StartTrafficLight()
        {
            if (IsTrafficLight)
            {
                m_objStartTime = TimeSpan.FromTicks(DateTime.Now.Ticks);
                m_intCurrentSecond = clsConfig.TrafficLightTimer;
                ChangeTimer.Start();
                m_objCountDownTimer.Start();
            }
        }

        public void StopTrafficLight()
        {
            if (IsTrafficLight)
            {
                ChangeTimer.Stop();
                m_objCountDownTimer.Stop();
            }
        }

        public void PauseTrafficLight()
        {
            if (IsTrafficLight)
            {
                m_objPauseTime = TimeSpan.FromTicks(DateTime.Now.Ticks);
                ChangeTimer.Stop();
                m_objAccelerateTimer.Stop();
                m_objCountDownTimer.Stop();
            }
        }
        #endregion

        #region Private Functions
        private void InitTrafficLight()
        {
            CurrentDirection = 0;
            m_intCurrentSecond = clsConfig.TrafficLightTimer;
            ChangeTimer = new DispatcherTimer();
            ChangeTimer.Interval = TimeSpan.FromMilliseconds((clsConfig.TrafficLightTimer * 1000) / clsConfig.SuperTime);
            ChangeTimer.Tick += ChangeTimer_Tick;
            TrafficLightVisual = new ctrlTrafficLight();
            CalculateCenterPoint();
            Canvas.SetLeft(TrafficLightVisual, TrafficLightPosition.X - clsConfig.TrafficLightSize / 2);
            Canvas.SetTop(TrafficLightVisual, TrafficLightPosition.Y - clsConfig.TrafficLightSize / 2);

            TrafficLightVisual.Angle = CalculateAngle();
        }

        private void RemoveTrafficLight()
        {
            CurrentDirection = 0;
            ChangeTimer.Stop();
            ChangeTimer.Tick -= ChangeTimer_Tick;
            ChangeTimer = null;
            TrafficLightVisual = new ctrlTrafficLight();
        }

        private void CalculateCenterPoint()
        {
            if (AdjacentChunks.Count == 0)
                return;

            bool _isDifferent = true;
            Point _objCenterPoint = _objCenterPoint = new Point(AdjacentChunks[0].Chunk.X1, AdjacentChunks[0].Chunk.Y1);

            for (int i = 1; i < AdjacentChunks.Count; i++)
            {
                if (AdjacentChunks[i].Chunk.X1 == _objCenterPoint.X && AdjacentChunks[i].Chunk.Y1 == _objCenterPoint.Y)
                    _isDifferent = false;
                else
                    if (AdjacentChunks[i].Chunk.X2 == _objCenterPoint.X && AdjacentChunks[i].Chunk.Y2 == _objCenterPoint.Y)
                        _isDifferent = false;
                    else
                        _isDifferent = true;
            }

            if (_isDifferent)
            {
                _objCenterPoint.X = AdjacentChunks[0].Chunk.X2;
                _objCenterPoint.Y = AdjacentChunks[0].Chunk.Y2;
            }

            TrafficLightPosition = _objCenterPoint;
        }

        private double CalculateAngle()
        {
            if (AdjacentChunks[CurrentDirection].Chunk.X1 != TrafficLightPosition.X && AdjacentChunks[CurrentDirection].Chunk.Y1 != TrafficLightPosition.Y)
                return AdjacentChunks[CurrentDirection].GetDegree(false);
            else
                return AdjacentChunks[CurrentDirection].GetDegree(true);
        }
        #endregion

        #region Event Handling
        private void ChangeTimer_Tick(object sender, EventArgs e)
        {
            m_objStartTime = TimeSpan.FromTicks(DateTime.Now.Ticks);
            m_objAccelerateTimer.Stop();
            m_objCountDownTimer.Stop();
            m_intCurrentSecond = clsConfig.TrafficLightTimer;

            ReInit();

            if (CurrentDirection < AdjacentChunks.Count - 1)
                CurrentDirection++;
            else
                CurrentDirection = 0;

            TrafficLightVisual.Angle = CalculateAngle();
            m_objCountDownTimer.Start();

            m_objAccelerateTimer.Start();

            clsGlobalStorage.TotalEvents++;
        }

        private void m_objAccelerateTimer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < AdjacentChunks[CurrentDirection].VehicleCollection.Count; i++)
            {
                if (AdjacentChunks[CurrentDirection].VehicleCollection[i].IsStopped)
                {
                    if (AdjacentChunks[CurrentDirection].VehicleCollection[i].ResumeMoving())
                        break;
                }
            }

            clsGlobalStorage.TotalEvents++;
        }
        #endregion

        #region Retired Pausing Functions
        //public void ResumeTrafficLight()
        //{
        //    if (IsTrafficLight)
        //    {
        //        TimeSpan _objSupposedTime = m_objStartTime + ChangeTimer.Interval;
        //        TimeSpan _objDiff = _objSupposedTime - m_objPauseTime;
        //        if (_objDiff.TotalMilliseconds <= 0)
        //            _objDiff = TimeSpan.FromMilliseconds(1);
        //        ChangeTimer.Interval = _objDiff;
        //        m_objStartTime = TimeSpan.FromTicks(DateTime.Now.Ticks);
        //        ChangeTimer.Start();
        //    }
        //}
        #endregion
    }
}
