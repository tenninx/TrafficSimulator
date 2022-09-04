using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;
using System.Windows.Media;
using System.Windows.Threading;
using TS.Library;
using TS.Map;
using TS.UI;
using TS.UI.Controls;

namespace TrafficSimulator
{
    public partial class winMain : Window
    {
        #region Public Properties
        public int CompletionCount { get; set; }
        #endregion

        #region Private Properties
        string m_strProgramVersion = "v1.0";
        int m_intCurrentID = 0;
        int m_intTotalMaxVehicle;
        int m_intRecklessCount = 0;
        double m_dblAverageCongestion = 0;
        int m_intSelectedMap = 1;
        bool m_isFirstRun = true;
        int m_intSecondsRun = 0;
        int m_intRealSecondsRun = 0;
        DispatcherTimer m_objSecondlyTimer, m_objAddVehicleTimer, m_objLocUpdateTimer, m_objAlwaysRealTimeSeconds;
        Random m_objVehicleTypeRandom, m_objRecknessRandom;
        List<clsChunk> m_objAllTLAdjacents = new List<clsChunk>();
        clsChunk m_objSelectedChunk;
        clsVehicle m_objSelectedVehicle;
        ctrlVehicle m_objVehicleLoc;
        object m_objAvgSpeedLocker, m_objPauseLocker;
        #endregion

        public winMain()
        {
            InitializeComponent();
            Title = "Traffic Simulator " + m_strProgramVersion;

            m_objPauseLocker = new object();

            CompletionCount = 0;
            clsGlobalStorage.TotalThreads += 4;

            m_objVehicleTypeRandom = new Random();
            m_objRecknessRandom = new Random();
            m_objAddVehicleTimer = new DispatcherTimer();
            m_objAddVehicleTimer.Interval = TimeSpan.FromMilliseconds(50);
            m_objAddVehicleTimer.Tick += new EventHandler(m_objAddVehicleTimer_Tick);

            objVehicleList.ItemsSource = clsGlobalStorage.AllVehicles;
            objCompleteList.ItemsSource = clsGlobalStorage.CompletedVehicles;

            clsGlobalStorage.AllVehicles.CollectionChanged += AllVehicles_CollectionChanged;

            m_objAvgSpeedLocker = new object();
            m_objSecondlyTimer = new DispatcherTimer();
            m_objSecondlyTimer.Interval = TimeSpan.FromMilliseconds(1000 / clsConfig.SuperTime);
            m_objSecondlyTimer.Tick += m_objAvgSpeedTimer_Tick;
            m_objSecondlyTimer.Start();

            m_objLocUpdateTimer = new DispatcherTimer();
            m_objLocUpdateTimer.Interval = TimeSpan.FromMilliseconds(500);
            m_objLocUpdateTimer.Tick += new EventHandler(m_objLocUpdateTimer_Tick);

            m_objAlwaysRealTimeSeconds = new DispatcherTimer();
            m_objAlwaysRealTimeSeconds.Interval = TimeSpan.FromMilliseconds(1000);
            m_objAlwaysRealTimeSeconds.Tick += new EventHandler(m_objAlwaysRealTimeSeconds_Tick);
            m_objAlwaysRealTimeSeconds.Start();
        }

        #region Form Load and Config
        private void winMain_Loaded(object sender, RoutedEventArgs e)
        {
            GenerateMap();
        }

        private void InitConfig()
        {
            objCfgVehicleCount.Minimum = 1;
            objCfgVehicleCount.Maximum = (int)(m_intTotalMaxVehicle / 2 * clsConfig.MaxAllowableCongestion);
            objCfgVehicleCount.ValueChanged += new RoutedPropertyChangedEventHandler<double>(objCfgVehicleCount_ValueChanged);
            objCfgVehicleCount.Value = 200;

            //objCfgMapScale.Minimum = 1;
            //objCfgMapScale.Maximum = 10;
            //objCfgMapScale.ValueChanged += new RoutedPropertyChangedEventHandler<double>(objCfgMapScale_ValueChanged);
            //objCfgMapScale.Value = 10;  // Just to trigger ValueChanged event
            //objCfgMapScale.Value = 2;

            objCfgTLTimer.Minimum = 5;
            objCfgTLTimer.Maximum = 30;
            objCfgTLTimer.ValueChanged += new RoutedPropertyChangedEventHandler<double>(objCfgTLTimer_ValueChanged);
            objCfgTLTimer.Value = 10;
        }

        private void ReInitVehicleCountConfig()
        {
            m_intTotalMaxVehicle = 0;
            for (int i = 0; i < clsGlobalStorage.AllChunks.Count; i++)
                m_intTotalMaxVehicle += clsGlobalStorage.AllChunks[i].MaxVehicleCount;
            objCfgVehicleCount.Maximum = (int)(m_intTotalMaxVehicle / 2 * clsConfig.MaxAllowableCongestion);
        }

        private void objCfgTLTimer_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            labCfgTLTimer.Text = objCfgTLTimer.Value.ToString();
            clsConfig.TrafficLightTimer = (int)objCfgTLTimer.Value;

            // Reinit Traffic Light Timer
            foreach (clsIntersection _objIntersection in clsGlobalStorage.AllIntersections)
                _objIntersection.ReInit();
        }

        private void objCfgMapScale_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //clsConfig.MeterPerPixel = (int)objCfgMapScale.Value;
            //labCfgMapScale.Text = objCfgMapScale.Value.ToString();

            // Recalculate Map Scale
            //foreach (clsChunk _objChunk in clsGlobalStorage.AllChunks)
            //    _objChunk.Init();
            //ReInitVehicleCountConfig();
        }

        private void objCfgVehicleCount_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            labCfgVehicleCount.Text = objCfgVehicleCount.Value.ToString();
        }
        #endregion

        #region Secondly Timer
        private void m_objAvgSpeedTimer_Tick(object sender, EventArgs e)
        {
            CalculateAverageSpeed();
            if (clsGlobalStorage.IsRunning)
            {
                m_intSecondsRun++;

                Title = "Traffic Simulator " + m_strProgramVersion;

                if (clsConfig.SuperTime > 1)
                    Title += " Super Time " + clsConfig.SuperTime + "x";
                Title += " Simulation Elapsed Time: " + TimeSpan.FromSeconds(m_intSecondsRun).ToString(@"mm\:ss");
                if (clsConfig.TerminateTimer >= 30)
                    Title += " / " + TimeSpan.FromSeconds(clsConfig.TerminateTimer).ToString(@"mm\:ss") + " (Timed Simulation)";
            }

            if (clsConfig.TerminateTimer != 0 && clsGlobalStorage.CompletedVehicles.Count > 0 && clsConfig.TerminateTimer == m_intSecondsRun)
                TerminateSimulation();

            clsGlobalStorage.TotalEvents++;
        }

        private void CalculateAverageSpeed()
        {
            lock (m_objAvgSpeedLocker)
            {
                double _dblTotalSpeed = 0;
                foreach (clsVehicle _objVehicle in clsGlobalStorage.AllVehicles)
                    _dblTotalSpeed += _objVehicle.CurrentSpeed;
                if (_dblTotalSpeed != 0)
                    labAvgSpeed.Text = String.Format("{0:0.00} km/h", _dblTotalSpeed / clsGlobalStorage.AllVehicles.Count);
                else
                    labAvgSpeed.Text = "0.00 km/h";
            }

            double _dblConRate = (double)clsGlobalStorage.AllVehicles.Count / (double)(m_intTotalMaxVehicle / 2);
            m_dblAverageCongestion += _dblConRate;
            m_dblAverageCongestion /= 2;
            m_dblAverageCongestion *= clsConfig.MaxAllowableCongestion;

            if (_dblConRate > 0)
                labCongestionRate.Text = _dblConRate.ToString("0.00 %");
            else
                labCongestionRate.Text = "0.00 %";
        }

        private void m_objAlwaysRealTimeSeconds_Tick(object sender, EventArgs e)
        {
            if (clsGlobalStorage.IsRunning)
                m_intRealSecondsRun++;

            clsGlobalStorage.TotalEvents++;
        }
        #endregion

        #region Map Generation
        private void SetTrafficLight(clsIntersection p_objIntersection, bool p_isTrafficLight)
        {
            p_objIntersection.IsTrafficLight = p_isTrafficLight;
            if (p_isTrafficLight)
            {
                pnlMap.Children.Add(p_objIntersection.TrafficLightVisual);
                for (int i = 0; i < p_objIntersection.AdjacentChunks.Count; i++)
                    m_objAllTLAdjacents.Add(p_objIntersection.AdjacentChunks[i]);
                p_objIntersection.StartTrafficLight();
                clsGlobalStorage.AllIntersections.Add(p_objIntersection);
                clsGlobalStorage.AllIntersections.AllTLPoints.Add(p_objIntersection.TrafficLightPosition);
            }
            else
            {
                p_objIntersection.StopTrafficLight();
                pnlMap.Children.Remove(p_objIntersection.TrafficLightVisual);
                for (int i = 0; i < p_objIntersection.AdjacentChunks.Count; i++)
                    m_objAllTLAdjacents.Remove(p_objIntersection.AdjacentChunks[i]);
                clsGlobalStorage.AllIntersections.Remove(p_objIntersection);
                clsGlobalStorage.AllIntersections.AllTLPoints.Remove(p_objIntersection.TrafficLightPosition);
            }
        }
        #endregion

        #region Vehicle Generation
        private void m_objAddVehicleTimer_Tick(object sender, EventArgs e)
        {
            // Set Vehicle Count
            if (clsGlobalStorage.AllVehicles.Count < objCfgVehicleCount.Value)
                GenerateNext();
            clsGlobalStorage.TotalEvents++;
        }

        private void GenerateNext()
        {
            //while (true)
            //{
            List<Chunk> _objRoute = Generator.GetRoute(m_intSelectedMap);
            if (clsGlobalStorage.AllChunks[_objRoute[0].ID].GetVehicleCount() < clsGlobalStorage.AllChunks[_objRoute[0].ID].MaxVehicleCount)
            {
                clsVehicle _objVehicle = GenerateVehicle();
                //_objVehicle.StartedMoving += new EventHandler(_objVehicle_StartedMoving);

                _objVehicle.ChunkCollection = new clsChunkCollection();
                foreach (Chunk _objChunk in _objRoute)
                {
                    _objVehicle.ChunkCollection.Add(clsGlobalStorage.AllChunks[_objChunk.ID]);
                    //_objVehicle.Route.Add(new clsPointSet() { SrcX = _objChunk.X1, SrcY = _objChunk.Y1, DesX = _objChunk.X2, DesY = _objChunk.Y2 });
                }

                SortRoute(_objVehicle);

                clsGlobalStorage.InsertVehicle(_objVehicle);

                _objVehicle.StartMoving();

                // Set Vehicle Count
                //Thread.Sleep(50);
                //if (clsGlobalStorage.AllVehicles.Count >= 100)
                //    break;
            }
            //}
        }

        // Sort the route as A -> B, B -> C, C -> D
        private void SortRoute(clsVehicle p_objVehicle)
        {
            if (p_objVehicle.ChunkCollection.Count > 1)
            {
                for (int i = 0; i < p_objVehicle.ChunkCollection.Count - 1; i++)
                {
                    if (p_objVehicle.ChunkCollection[i].Chunk.X2 == p_objVehicle.ChunkCollection[i + 1].Chunk.X1 && p_objVehicle.ChunkCollection[i].Chunk.Y2 == p_objVehicle.ChunkCollection[i + 1].Chunk.Y1)
                    {
                        p_objVehicle.Route.Add(new clsPointSet()
                        {
                            X2 = p_objVehicle.ChunkCollection[i].Chunk.X2,
                            Y2 = p_objVehicle.ChunkCollection[i].Chunk.Y2,
                            X1 = p_objVehicle.ChunkCollection[i].Chunk.X1,
                            Y1 = p_objVehicle.ChunkCollection[i].Chunk.Y1
                        });
                    }
                    else
                    {
                        p_objVehicle.Route.Add(new clsPointSet()
                        {
                            X2 = p_objVehicle.ChunkCollection[i].Chunk.X1,
                            Y2 = p_objVehicle.ChunkCollection[i].Chunk.Y1,
                            X1 = p_objVehicle.ChunkCollection[i].Chunk.X2,
                            Y1 = p_objVehicle.ChunkCollection[i].Chunk.Y2
                        });
                    }
                }

                if (p_objVehicle.Route[p_objVehicle.Route.Count - 1].X2 == p_objVehicle.ChunkCollection[p_objVehicle.ChunkCollection.Count - 1].Chunk.X1 &&
                    p_objVehicle.Route[p_objVehicle.Route.Count - 1].Y2 == p_objVehicle.ChunkCollection[p_objVehicle.ChunkCollection.Count - 1].Chunk.Y1)
                    p_objVehicle.Route.Add(new clsPointSet()
                    {
                        X2 = p_objVehicle.ChunkCollection[p_objVehicle.ChunkCollection.Count - 1].Chunk.X2,
                        Y2 = p_objVehicle.ChunkCollection[p_objVehicle.ChunkCollection.Count - 1].Chunk.Y2,
                        X1 = p_objVehicle.ChunkCollection[p_objVehicle.ChunkCollection.Count - 1].Chunk.X1,
                        Y1 = p_objVehicle.ChunkCollection[p_objVehicle.ChunkCollection.Count - 1].Chunk.Y1
                    });
                else
                {
                    p_objVehicle.Route.Add(new clsPointSet()
                    {
                        X2 = p_objVehicle.ChunkCollection[p_objVehicle.ChunkCollection.Count - 1].Chunk.X1,
                        Y2 = p_objVehicle.ChunkCollection[p_objVehicle.ChunkCollection.Count - 1].Chunk.Y1,
                        X1 = p_objVehicle.ChunkCollection[p_objVehicle.ChunkCollection.Count - 1].Chunk.X2,
                        Y1 = p_objVehicle.ChunkCollection[p_objVehicle.ChunkCollection.Count - 1].Chunk.Y2
                    });
                }
            }
            else
                p_objVehicle.Route.Add(new clsPointSet()
                {
                    X1 = p_objVehicle.ChunkCollection[0].Chunk.X1,
                    Y1 = p_objVehicle.ChunkCollection[0].Chunk.Y1,
                    X2 = p_objVehicle.ChunkCollection[0].Chunk.X2,
                    Y2 = p_objVehicle.ChunkCollection[0].Chunk.Y2
                });
        }

        //private void _objVehicle_StartedMoving(object sender, EventArgs e)
        //{
        //    clsVehicle _objVehicle = (clsVehicle)sender;
        //    _objVehicle.StartedMoving -= _objVehicle_StartedMoving;

        //    if (clsGlobalStorage.AllVehicles.Count < 100)
        //        GenerateNext();
        //}

        private clsVehicle GenerateVehicle()
        {
            int _intType = m_objVehicleTypeRandom.Next(1, 31);
            int _intNotReck = m_objRecknessRandom.Next(1, 101);

            clsVehicle _objVehicle = new clsVehicle();
            _objVehicle.JourneyCompleted += _objVehicle_JourneyCompleted;

            // 3 out of 30 is Bus
            if (_intType < 4)
            {
                _objVehicle.VehicleType = enumVehicleType.Bus;
                _objVehicle.MaxSpeed = 50;
            }
            // 2 out of 30 is Lorry
            else if (_intType < 6)
            {
                _objVehicle.VehicleType = enumVehicleType.Lorry;
                _objVehicle.MaxSpeed = 40;
            }
            // the rest is Car
            else
            {
                _objVehicle.VehicleType = enumVehicleType.Car;
                _objVehicle.MaxSpeed = 70;
            }

            _objVehicle.VehicleID = m_intCurrentID++;
            if (_intNotReck > clsConfig.RecklessPercentage)
                _objVehicle.Reckless = false;
            else
            {
                _objVehicle.Reckless = true;
                m_intRecklessCount++;
            }

            clsGlobalStorage.TotalThreads += 2;

            return _objVehicle;
        }
        #endregion

        #region Event Handling
        private void _objVehicle_JourneyCompleted(object sender, EventArgs e)
        {
            clsVehicle _objVehicle = (clsVehicle)sender;
            _objVehicle.JourneyCompleted -= _objVehicle_JourneyCompleted;
            _objVehicle.ChunkCollection[_objVehicle.ChunkCollection.Count - 1].ShowOutline(false);
            CompletionCount++;

            btnTerminate.IsEnabled = true;

            if (_objVehicle.Reckless)
                m_intRecklessCount--;

            labCompleted.Text = CompletionCount.ToString();

            clsGlobalStorage.TotalEvents++;
        }

        private void AllVehicles_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            labTotalVehicle.Text = clsGlobalStorage.AllVehicles.Count.ToString() + " ( " + m_intRecklessCount + " )";
            clsGlobalStorage.TotalEvents++;
        }

        #region Button Controls
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (btnStart.Content.ToString() == "Start")
            {
                if (m_isFirstRun)
                {
                    m_isFirstRun = false;
                    clsGlobalStorage.IsRunning = true;
                }
                btnStart.Content = "Stop";
                btnAdvConfig.IsEnabled = false;
                pnlMapSelection.IsEnabled = false;
                if (clsGlobalStorage.IsRunning)
                    m_objAddVehicleTimer.Start();
            }
            else
            {
                m_objAddVehicleTimer.Stop();
                btnStart.Content = "Start";
            }
        }

        // Simulate Straight Line Traffic Light Movement Only
        private void btnSimulateTL_Click(object sender, RoutedEventArgs e)
        {
            if (m_isFirstRun)
            {
                m_isFirstRun = false;
                clsGlobalStorage.IsRunning = true;
            }

            btnAdvConfig.IsEnabled = false;
            pnlMapSelection.IsEnabled = false;

            if (clsGlobalStorage.AllChunks[0].GetVehicleCount() < clsGlobalStorage.AllChunks[0].MaxVehicleCount)
            {
                clsVehicle _objVehicle = GenerateVehicle();
                _objVehicle.VehicleType = enumVehicleType.Car;
                _objVehicle.ChunkCollection = new clsChunkCollection();

                _objVehicle.ChunkCollection.Add(clsGlobalStorage.AllChunks[0]);
                _objVehicle.ChunkCollection.Add(clsGlobalStorage.AllChunks[1]);
                _objVehicle.ChunkCollection.Add(clsGlobalStorage.AllChunks[2]);
                _objVehicle.ChunkCollection.Add(clsGlobalStorage.AllChunks[3]);
                _objVehicle.ChunkCollection.Add(clsGlobalStorage.AllChunks[4]);
                _objVehicle.ChunkCollection.Add(clsGlobalStorage.AllChunks[5]);
                _objVehicle.ChunkCollection.Add(clsGlobalStorage.AllChunks[6]);
                _objVehicle.ChunkCollection.Add(clsGlobalStorage.AllChunks[7]);
                _objVehicle.ChunkCollection.Add(clsGlobalStorage.AllChunks[31]);
                _objVehicle.ChunkCollection.Add(clsGlobalStorage.AllChunks[32]);
                _objVehicle.ChunkCollection.Add(clsGlobalStorage.AllChunks[33]);
                _objVehicle.ChunkCollection.Add(clsGlobalStorage.AllChunks[34]);
                _objVehicle.ChunkCollection.Add(clsGlobalStorage.AllChunks[29]);
                _objVehicle.ChunkCollection.Add(clsGlobalStorage.AllChunks[30]);

                SortRoute(_objVehicle);

                clsGlobalStorage.InsertVehicle(_objVehicle);

                _objVehicle.StartMoving();

                objVehicleList.SelectedItem = _objVehicle;
            }
        }

        private void btnAddReckless_Click(object sender, RoutedEventArgs e)
        {
            if (m_isFirstRun)
            {
                m_isFirstRun = false;
                clsGlobalStorage.IsRunning = true;
            }

            btnAdvConfig.IsEnabled = false;
            pnlMapSelection.IsEnabled = false;

            if (clsGlobalStorage.AllChunks[0].GetVehicleCount() < clsGlobalStorage.AllChunks[0].MaxVehicleCount)
            {
                clsVehicle _objVehicle = GenerateVehicle();
                if (!_objVehicle.Reckless)
                    m_intRecklessCount++;
                _objVehicle.Reckless = true;
                _objVehicle.ChunkCollection = new clsChunkCollection();

                _objVehicle.ChunkCollection.Add(clsGlobalStorage.AllChunks[0]);
                _objVehicle.ChunkCollection.Add(clsGlobalStorage.AllChunks[1]);
                _objVehicle.ChunkCollection.Add(clsGlobalStorage.AllChunks[2]);
                _objVehicle.ChunkCollection.Add(clsGlobalStorage.AllChunks[3]);
                _objVehicle.ChunkCollection.Add(clsGlobalStorage.AllChunks[4]);
                _objVehicle.ChunkCollection.Add(clsGlobalStorage.AllChunks[5]);
                _objVehicle.ChunkCollection.Add(clsGlobalStorage.AllChunks[6]);
                _objVehicle.ChunkCollection.Add(clsGlobalStorage.AllChunks[7]);
                _objVehicle.ChunkCollection.Add(clsGlobalStorage.AllChunks[8]);
                _objVehicle.ChunkCollection.Add(clsGlobalStorage.AllChunks[9]);
                _objVehicle.ChunkCollection.Add(clsGlobalStorage.AllChunks[10]);
                _objVehicle.ChunkCollection.Add(clsGlobalStorage.AllChunks[11]);
                _objVehicle.ChunkCollection.Add(clsGlobalStorage.AllChunks[12]);

                SortRoute(_objVehicle);

                clsGlobalStorage.InsertVehicle(_objVehicle);

                _objVehicle.StartMoving();

                objVehicleList.SelectedItem = _objVehicle;
            }
        }

        private void btnAdvConfig_Click(object sender, RoutedEventArgs e)
        {
            winAdvConfig _objAdvConfig = new winAdvConfig();
            _objAdvConfig.Owner = this;
            _objAdvConfig.Closed += new EventHandler(_objAdvConfig_Closed);
            _objAdvConfig.ShowDialog();
        }

        private void _objAdvConfig_Closed(object sender, EventArgs e)
        {
            foreach (clsChunk _objChunk in clsGlobalStorage.AllChunks)
                _objChunk.Init();
            foreach (clsIntersection _objIntersection in clsGlobalStorage.AllIntersections)
                _objIntersection.ReInit();
            ReInitVehicleCountConfig();
            m_objSecondlyTimer.Stop();
            m_objSecondlyTimer.Interval = TimeSpan.FromMilliseconds(1000 / clsConfig.SuperTime);
            m_objSecondlyTimer.Start();
            m_objAddVehicleTimer.Interval = TimeSpan.FromMilliseconds(50 / clsConfig.SuperTime);
        }

        private void btnTerminate_Click(object sender, RoutedEventArgs e)
        {
            TerminateSimulation();
        }

        private void cboxShowReckless_Checked(object sender, RoutedEventArgs e)
        {
            clsGlobalStorage.BlinkReckless = cboxShowReckless.IsChecked.Value;
        }

        private void cboxHighlightSelection_Checked(object sender, RoutedEventArgs e)
        {
            clsGlobalStorage.HighlightSelection = cboxHighlightSelection.IsChecked.Value;
        }

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            winAbout _objAbout = new winAbout();
            _objAbout.Owner = this;
            _objAbout.ShowDialog();
        }

        private void rbtnMap1_Click(object sender, RoutedEventArgs e)
        {
            m_intSelectedMap = 0;
            GenerateMap();

            btnAddReckless.IsEnabled = false;
            btnSimulateTL.IsEnabled = false;
        }

        private void rbtnMap2_Click(object sender, RoutedEventArgs e)
        {
            m_intSelectedMap = 1;
            GenerateMap();

            btnAddReckless.IsEnabled = true;
            btnSimulateTL.IsEnabled = true;
        }

        private void rbtnMap3_Click(object sender, RoutedEventArgs e)
        {
            m_intSelectedMap = 2;
            GenerateMap();

            btnAddReckless.IsEnabled = false;
            btnSimulateTL.IsEnabled = false;
        }
        #endregion

        #region Vehicle Location
        private void objVehicleList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            m_objLocUpdateTimer.Stop();

            if (e.AddedItems.Count > 0)
            {
                clsVehicle _objVehicle = e.AddedItems[0] as clsVehicle;

                if (m_objSelectedChunk != null && m_objSelectedVehicle != _objVehicle)
                {
                    m_objSelectedVehicle.ChunkChanged -= _objVehicle_ChunkChanged;
                    m_objSelectedChunk.ShowOutline(false);
                }

                m_objSelectedChunk = _objVehicle.ChunkCollection[_objVehicle.CurrentPosition];
                m_objSelectedChunk.ShowOutline(true);
                _objVehicle.ChunkChanged += new EventHandler(_objVehicle_ChunkChanged);

                m_objLocUpdateTimer.Start();

                m_objSelectedVehicle = _objVehicle;
            }
            else
                m_objVehicleLoc.Visibility = Visibility.Collapsed;
        }

        private void m_objLocUpdateTimer_Tick(object sender, EventArgs e)
        {
            Point _objCurrentPoint = m_objSelectedVehicle.GetCurrentPosition();

            Canvas.SetLeft(m_objVehicleLoc, _objCurrentPoint.X - clsConfig.AdjustX);
            Canvas.SetTop(m_objVehicleLoc, _objCurrentPoint.Y - clsConfig.AdjustY);
            m_objVehicleLoc.Speed = m_objSelectedVehicle.CurrentSpeed.ToString("F0");
            m_objVehicleLoc.Visibility = Visibility.Visible;

            clsGlobalStorage.TotalEvents++;
        }

        private void _objVehicle_ChunkChanged(object sender, EventArgs e)
        {
            if (m_objSelectedChunk != null)
                m_objSelectedChunk.ShowOutline(false);

            clsVehicle _objVehicle = sender as clsVehicle;
            m_objSelectedChunk = _objVehicle.ChunkCollection[_objVehicle.CurrentPosition];
            m_objSelectedChunk.ShowOutline(true);

            clsGlobalStorage.TotalEvents++;
        }
        #endregion

        #endregion

        #region Private Functions
        private void GenerateMap()
        {
            clsGlobalStorage.AllChunks.Clear();
            clsGlobalStorage.AllIntersections.Clear();
            pnlMap.Children.Clear();

            List<Chunk> _objChunks = Generator.GetMap(m_intSelectedMap);

            m_intTotalMaxVehicle = 0;

            foreach (Chunk _objCurrentChunk in _objChunks)
            {
                clsChunk _objNew = new clsChunk(_objCurrentChunk.ID);
                _objNew.SetLocation(_objCurrentChunk.X1, _objCurrentChunk.Y1, _objCurrentChunk.X2, _objCurrentChunk.Y2);
                _objNew.SpeedLimit = _objCurrentChunk.SpeedLimit;
                clsGlobalStorage.InsertChunk(_objNew);
                m_intTotalMaxVehicle += _objNew.MaxVehicleCount;
                pnlMap.Children.Add(_objNew.Chunk);
            }

            m_objVehicleLoc = new ctrlVehicle();
            m_objVehicleLoc.Effect = new DropShadowEffect() { Color = Colors.Black, Opacity = 1, ShadowDepth = 0, BlurRadius = 10 };
            m_objVehicleLoc.Visibility = Visibility.Collapsed;
            pnlMap.Children.Add(m_objVehicleLoc);
            
            List<Intersection> _objIntersections = Generator.GetIntersection(m_intSelectedMap);

            foreach (Intersection _objIntersection in _objIntersections)
            {
                clsIntersection _objInter = new clsIntersection();
                foreach (Chunk a in _objIntersection.Chunks)
                    _objInter.InsertAdjacentChunk(a.ID);
                SetTrafficLight(_objInter, true);
            }

            InitConfig();
        }

        private void TerminateSimulation()
        {
            m_objAddVehicleTimer.Stop();
            m_objSecondlyTimer.Stop();
            m_objLocUpdateTimer.Stop();
            m_objAlwaysRealTimeSeconds.Stop();

            for (int i = 0; i < clsGlobalStorage.AllVehicles.Count; i++)
                clsGlobalStorage.AllVehicles[i].PauseTimer();

            for (int i = 0; i < clsGlobalStorage.AllIntersections.Count; i++)
                clsGlobalStorage.AllIntersections[i].PauseTrafficLight();

            clsReport _objReportStats = new clsReport();
            _objReportStats.SecondsRun = m_intSecondsRun;
            _objReportStats.RealSecondsRun = m_intRealSecondsRun;
            _objReportStats.TotalVehicles = clsGlobalStorage.CompletedVehicles.Count + clsGlobalStorage.AllVehicles.Count;
            _objReportStats.TotalReckless = 0;
            _objReportStats.SlowedVehicles = clsGlobalStorage.SlowedVehicles.Count;

            double _dblAvgJourneyTime = 0;
            double _dblAvgSpeed = 0;

            for (int i = 0; i < clsGlobalStorage.CompletedVehicles.Count; i++)
            {
                if (clsGlobalStorage.CompletedVehicles[i].Reckless)
                    _objReportStats.TotalReckless++;
                _dblAvgJourneyTime += clsGlobalStorage.CompletedVehicles[i].CompleteTime.TotalMilliseconds;
                _dblAvgSpeed += clsGlobalStorage.CompletedVehicles[i].AverageSpeed;
            }

            _objReportStats.TotalCompleted = clsGlobalStorage.CompletedVehicles.Count;
            _objReportStats.AverageJourneyTime = TimeSpan.FromMilliseconds(_dblAvgJourneyTime / clsGlobalStorage.CompletedVehicles.Count);
            _objReportStats.AverageSpeed = _dblAvgSpeed / clsGlobalStorage.CompletedVehicles.Count;
            _objReportStats.CongestionRate = m_dblAverageCongestion;
            _objReportStats.TotalThreads = clsGlobalStorage.TotalThreads;
            _objReportStats.TotalEvents = clsGlobalStorage.TotalEvents;

            btnAddReckless.IsEnabled = false;
            btnSimulateTL.IsEnabled = false;
            btnStart.IsEnabled = false;

            winReport _objReport = new winReport(_objReportStats);
            _objReport.Owner = this;
            _objReport.ShowDialog();
        }
        #endregion

        #region Retired Pausing Functions
        //private void btnPause_Click(object sender, RoutedEventArgs e)
        //{
        //    return;

        //    if (btnPause.Content.ToString() == "Pause")
        //    {
        //        btnPause.Content = "Resume";
        //        clsGlobalStorage.IsRunning = false;
        //        PauseAddVehicleTimer();

        //        lock (m_objPauseLocker)
        //        {
        //            for (int i = 0; i < clsGlobalStorage.AllVehicles.Count; i++)
        //                clsGlobalStorage.AllVehicles[i].PauseTimer();

        //            for (int i = 0; i < clsGlobalStorage.AllIntersections.Count; i++)
        //                clsGlobalStorage.AllIntersections[i].PauseTrafficLight();
        //        }

        //        objChunkCompleted.Visibility = Visibility.Visible;
        //        if (objVehicleList.SelectedItem != null)
        //            UpdateVehicleLocation((clsVehicle)objVehicleList.SelectedItem);
        //    }
        //    else
        //    {
        //        objChunkCompleted.Visibility = Visibility.Collapsed;

        //        lock (m_objPauseLocker)
        //        {
        //            ResumeAddVehicleTimer();

        //            for (int i = 0; i < clsGlobalStorage.AllVehicles.Count; i++)
        //                clsGlobalStorage.AllVehicles[i].ResumeTimer();

        //            for (int i = 0; i < clsGlobalStorage.AllIntersections.Count; i++)
        //                clsGlobalStorage.AllIntersections[i].ResumeTrafficLight();
        //        }

        //        clsGlobalStorage.IsRunning = true;
        //        m_objVehicleLoc.Visibility = Visibility.Collapsed;
        //        btnPause.Content = "Pause";
        //    }
        //}

        //private void UpdateVehicleLocation(clsVehicle p_objSelectedVehicle)
        //{
        //    clsPointSet _objRoute = p_objSelectedVehicle.Route[p_objSelectedVehicle.CurrentPosition];
        //    double _dblDistX = _objRoute.X1 - _objRoute.X2;
        //    double _dblDistY = _objRoute.Y1 - _objRoute.Y2;

        //    Point _objVehicleLoc = new Point();
        //    if (_dblDistX < 0)
        //        _objVehicleLoc.X = _objRoute.X1 + (Math.Abs(_dblDistX) * p_objSelectedVehicle.ChunkCompleted / 100);
        //    else
        //        _objVehicleLoc.X = _objRoute.X1 - (_dblDistX * p_objSelectedVehicle.ChunkCompleted / 100);
        //    if (_dblDistY < 0)
        //        _objVehicleLoc.Y = _objRoute.Y1 + (Math.Abs(_dblDistY) * p_objSelectedVehicle.ChunkCompleted / 100);
        //    else
        //        _objVehicleLoc.Y = _objRoute.Y1 - (_dblDistY * p_objSelectedVehicle.ChunkCompleted / 100);

        //    Canvas.SetLeft(m_objVehicleLoc, _objVehicleLoc.X - clsConfig.AdjustX);
        //    Canvas.SetTop(m_objVehicleLoc, _objVehicleLoc.Y - clsConfig.AdjustY);
        //    m_objVehicleLoc.Speed = p_objSelectedVehicle.CurrentSpeed.ToString();
        //    m_objVehicleLoc.Visibility = Visibility.Visible;
        //}

        //private void PauseAddVehicleTimer()
        //{
        //    m_objAddVehicleTimer.Stop();
        //}

        //private void ResumeAddVehicleTimer()
        //{
        //    if (btnStart.Content.ToString() == "Stop")
        //        m_objAddVehicleTimer.Start();
        //    else
        //        m_objAddVehicleTimer.Stop();
        //}
        #endregion
    }
}
