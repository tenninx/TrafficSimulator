using System;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Effects;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Animation;

namespace TS.Library
{
    public class clsChunk
    {
        public Line Chunk { get; set; }
        public int ChunkID { get; set; }
        public double SpeedLimit { get; set; }
        public double RealLength { get; set; }
        public int MaxVehicleCount { get; set; }
        public int NonCongestedCount { get; set; }
        public clsVehicleCollection VehicleCollection { get; set; }

        public event EventHandler VehicleRemoved;
        public int RecklessCount;

        object m_objLocker = new object();

        SolidColorBrush m_objDefaultColor = new SolidColorBrush(new Color() { A = 255, R = 0, G = 255, B = 0 });
        Color m_objCurrentColor = new Color();
        Storyboard m_objBlinkStoryboard = new Storyboard();

        public clsChunk(int p_intChunkID)
        {
            ChunkID = p_intChunkID;
            SpeedLimit = 60;
            RecklessCount = 0;
            VehicleCollection = new clsVehicleCollection();
            VehicleCollection.CollectionChanged += VehicleCollection_CollectionChanged;
        }

        #region Public Functions
        public double GetChunkLength()
        {
            double _dblWidth = Math.Abs(Chunk.X2 - Chunk.X1);
            double _dblHeight = Math.Abs(Chunk.Y2 - Chunk.Y1);
            return Math.Sqrt(_dblWidth * _dblWidth + _dblHeight * _dblHeight);
        }

        public void SetLocation(double p_dblX1, double p_dblY1, double p_dblX2, double p_dblY2)
        {
            Chunk = new Line();
            Chunk.StrokeThickness = clsConfig.ChunkThickness;
            Chunk.Stroke = m_objDefaultColor;

            Chunk.X1 = p_dblX1;
            Chunk.Y1 = p_dblY1;
            Chunk.X2 = p_dblX2;
            Chunk.Y2 = p_dblY2;
            Chunk.ToolTip = ChunkID;
            Chunk.ToolTipOpening += Chunk_ToolTipOpening;

            InitEffect();
            Init();
        }

        public void Blink(bool p_isBlinking)
        {
            if (p_isBlinking)
                m_objBlinkStoryboard.Begin();
            else
                m_objBlinkStoryboard.Stop();
        }

        public double GetDegree(bool p_isReverse)
        {
            if (!p_isReverse)
                return Math.Atan2(Chunk.Y2 - Chunk.Y1, Chunk.X2 - Chunk.X1) * (180.0 / Math.PI);
            else
                return Math.Atan2(Chunk.Y1 - Chunk.Y2, Chunk.X1 - Chunk.X2) * (180.0 / Math.PI);
        }

        public void ShowOutline(bool p_isShown)
        {
            if (clsGlobalStorage.HighlightSelection && RecklessCount <= 0 && p_isShown)
            {
                Chunk.Effect = new DropShadowEffect()
                {
                    Color = Colors.BlueViolet,
                    ShadowDepth = 0,
                    Opacity = 1,
                    BlurRadius = 20
                };
            }
            else if (RecklessCount <= 0)
            {
                Chunk.Effect = null;
            }
            else
                ShowRecklessOutline(true);
        }

        public void ShowRecklessOutline(bool p_isShown)
        {
            if (p_isShown)
            {
                Chunk.Effect = new DropShadowEffect()
                {
                    Color = Colors.Orange,
                    ShadowDepth = 0,
                    Opacity = 1,
                    BlurRadius = 20
                };
            }
            else
                Chunk.Effect = null;
        }

        public void Init()
        {
            RealLength = GetChunkLength() * clsConfig.MeterPerPixel;

            MaxVehicleCount = (int)(RealLength / clsConfig.VehicleLength);

            NonCongestedCount = (int)(MaxVehicleCount * clsConfig.NonCongestedPercent);
        }

        public double GetAverageSpeed()
        {
            return 0;
        }

        public int GetVehicleCount()
        {
            return VehicleCollection.Count;
        }

        public void InsertVehicle(clsVehicle p_objVehicle)
        {
            lock (m_objLocker)
            {
                VehicleCollection.Add(p_objVehicle);
                if (p_objVehicle.Reckless)
                {
                    RecklessCount++;
                    if (clsGlobalStorage.BlinkReckless)
                    {
                        Blink(true);
                        ShowRecklessOutline(true);
                    }
                }
            }
        }

        public void RemoveVehicle(clsVehicle p_objVehicle)
        {
            lock (m_objLocker)
            {
                VehicleCollection.Remove(p_objVehicle);
                if (p_objVehicle.Reckless)
                {
                    RecklessCount--;
                    if (RecklessCount <= 0)
                    {
                        Blink(false);
                        ShowRecklessOutline(false);
                    }
                }
                if (VehicleRemoved != null)
                    VehicleRemoved(this, new EventArgs());
            }
        }
        #endregion

        #region Private Functions
        private void InitEffect()
        {
            DoubleAnimationUsingKeyFrames _objKeyFrames = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTargetProperty(_objKeyFrames, new PropertyPath("(UIElement.Opacity)"));
            EasingDoubleKeyFrame _objKeyTime = new EasingDoubleKeyFrame();
            _objKeyTime.Value = 0.5;
            _objKeyTime.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(300));
            _objKeyFrames.KeyFrames.Add(_objKeyTime);
            _objKeyTime = new EasingDoubleKeyFrame();
            _objKeyTime.Value = 1;
            _objKeyTime.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(600));
            _objKeyFrames.KeyFrames.Add(_objKeyTime);

            m_objBlinkStoryboard.Children.Add(_objKeyFrames);
            Storyboard.SetTarget(_objKeyFrames, Chunk);
            m_objBlinkStoryboard.RepeatBehavior = RepeatBehavior.Forever;
        }
        #endregion

        #region Event Handling
        private void VehicleCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            double _dblColorDepth = (double)VehicleCollection.Count / (double)(MaxVehicleCount * 0.8);
            double _dblBlueDepth = 0;
            if (_dblColorDepth > 1)
            {
                _dblBlueDepth = _dblColorDepth - 1;
                _dblColorDepth = 1;
            }
            int _RedDepth = (int)(_dblColorDepth * 255);
            int _GreenDepth = 255 - _RedDepth;
            m_objCurrentColor.B = (byte)(_dblBlueDepth * 255);
            m_objCurrentColor.R = (byte)_RedDepth;
            m_objCurrentColor.G = (byte)_GreenDepth;
            m_objCurrentColor.A = 255;

            Chunk.Stroke = new SolidColorBrush(m_objCurrentColor);

            clsGlobalStorage.TotalEvents++;
        }

        private void Chunk_ToolTipOpening(object sender, System.Windows.Controls.ToolTipEventArgs e)
        {
            ToolTipService.SetShowDuration(Chunk, 10000);
            Chunk.ToolTip =
                "Chunk ID\t: " + ChunkID +
                "\nSpeed Limit\t: " + SpeedLimit +
                "\nLength\t\t: " + RealLength.ToString("0.00m") +
                "\nMax Vehicle\t: " + MaxVehicleCount +
                "\nVehicle Count\t: " + VehicleCollection.Count;
        }
        #endregion
    }
}
