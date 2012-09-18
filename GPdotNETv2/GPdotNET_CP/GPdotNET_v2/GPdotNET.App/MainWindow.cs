﻿//////////////////////////////////////////////////////////////////////////////////////////
// GPdotNET Tree based genetic programming tool                                         //
// Copyright 2006-2012 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the GNU Library General Public License (LGPL)       //
// See licence section of  http://gpdotnet.codeplex.com/license                         //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 //
// Bihac,Bosnia and Herzegovina                                                         //
// http://bhrnjica.wordpress.com                                                        //
//////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GPdotNET.Tool.Common;
using GPdotNET.Engine;

namespace GPdotNET.App
{
    public partial class MainWindow : Form
    {
        DataPanel               _dataPanel;
        FunctionPanel           _funPanel;
        SettingsPanel           _setPanel;
        AnaliticFunctionDef     _funDefinit;
        RunPanel                _runPanel;
        OptimizePanel           _optimizePanel;
        ResultPanel             _resultPanel;
        PreditionPanel          _predictionPanel;
        GPModelType             _GPModel;

        GPFactory               _gpFactory;
        GPFactory               _gaFactory;
        int                     _runningEngine = 0;

        int                     _bgpRuning = 0;

        bool                    _bModified = true;
        string                  _filePath = "";
        
       
        public MainWindow()
        {
            InitializeComponent();
            Load += new EventHandler(MainWindow_Load);
            this.Icon = Utility.LoadIconFromName("GPdotNET.App.Resources.gpdotnet_ico48.ico");
            this.logoPictureBox.Image = Utility.LoadImageFromName("GPdotNET.App.Resources.gp256V.png");
            
        }
     
		void LoadModelWizard(GPModelType model )
        {

            _GPModel = model;

            switch (_GPModel)
            {
                case GPModelType.SymbolicRegression:
                    {
                        _dataPanel = new DataPanel();
                        loadGPPanelInMainWindow(this, _dataPanel, "Load Data");
                        _dataPanel.SetTimeSeries(false);

                        _funPanel = new FunctionPanel();
                        loadGPPanelInMainWindow(this, _funPanel, "Functions");

                        _setPanel = new SettingsPanel();
                        loadGPPanelInMainWindow(this, _setPanel, "Settings");

                        _runPanel = new RunPanel();
                        loadGPPanelInMainWindow(this, _runPanel, "Run");

                        _resultPanel = new ResultPanel();
                        loadGPPanelInMainWindow(this, _resultPanel, "Result");

                        this._gpFactory = new GPFactory();
                        this._gpFactory.ReportEvolution += new Engine.EvolutionHandler(gpFactory_ReportEvolution);
                                              

                    }
                    break;
                case GPModelType.SymbolicRegressionWithOptimization:
                    {
                        _dataPanel = new DataPanel();
                        loadGPPanelInMainWindow(this, _dataPanel, "Load Data");
                        _dataPanel.SetTimeSeries(false);

                        _funPanel = new FunctionPanel();
                        loadGPPanelInMainWindow(this, _funPanel, "Functions");

                        _setPanel = new SettingsPanel();
                        loadGPPanelInMainWindow(this, _setPanel, "Settings");

                        _runPanel = new RunPanel();
                        loadGPPanelInMainWindow(this, _runPanel, "Run");


                        _optimizePanel = new OptimizePanel();
                        loadGPPanelInMainWindow(this, _optimizePanel, "Optimize Model");

                        _resultPanel = new ResultPanel();
                        loadGPPanelInMainWindow(this, _resultPanel, "Result");

                        this._gpFactory = new GPFactory();
                        this._gpFactory.ReportEvolution += new Engine.EvolutionHandler(gpFactory_ReportEvolution);
                        this._gaFactory = new GPFactory();
                        this._gaFactory.ReportEvolution += new Engine.EvolutionHandler(gpFactory_ReportEvolution);

                     
                    }
                    break;
                case GPModelType.TimeSeries:
                    {
                        _dataPanel = new DataPanel();
                        loadGPPanelInMainWindow(this, _dataPanel, "Load Data");
                        _dataPanel.SetTimeSeries(true);

                        _funPanel = new FunctionPanel();
                        loadGPPanelInMainWindow(this, _funPanel, "Functions");

                        _setPanel = new SettingsPanel();
                        loadGPPanelInMainWindow(this, _setPanel, "Settings");
                        _setPanel.SetNumOfConstance(0);

                        _runPanel = new RunPanel();
                        loadGPPanelInMainWindow(this, _runPanel, "Run");

                        _resultPanel = new ResultPanel();
                        loadGPPanelInMainWindow(this, _resultPanel, "Result");

                        this._gpFactory = new GPFactory();
                        this._gpFactory.ReportEvolution += new Engine.EvolutionHandler(gpFactory_ReportEvolution);
                       

                    }
                    break;
                case GPModelType.AnaliticFunctionOptimization:
                    {
                     
                        _funDefinit = new AnaliticFunctionDef();
                        loadGPPanelInMainWindow(this, _funDefinit, "Analytic  function");

                        _funPanel = new FunctionPanel();
                        loadGPPanelInMainWindow(this, _funPanel, "Functions");
                        
                        _setPanel = new SettingsPanel();
                        loadGPPanelInMainWindow(this, _setPanel, "Settings");

                        _optimizePanel = new OptimizePanel();
                        loadGPPanelInMainWindow(this, _optimizePanel, "Optimize Model");


                        //initi pages after creation
                        //
                        _funDefinit.LoadFuns(_funPanel.GPFunctions);
                        tabControl1.TabPages.RemoveAt(1);

                        this._gaFactory = new GPFactory();
                        this._gaFactory.ReportEvolution += new Engine.EvolutionHandler(gpFactory_ReportEvolution);
                    }
                    break;
                default:
                    break;
            }

            //Evaents from datapanel about loading dat
            if (_dataPanel != null)
            {
                _dataPanel.DataLoaded += (x, y) =>
                        {
                            if (_runPanel != null)
                            {
                                _runPanel.UpdateChartDataPoint(_dataPanel.GetOutputValues(),false);
                            }

                        };
            }

            if (_funDefinit != null)
            {
                _funDefinit.btnFinishAnalFun.Click += (xx, yy) =>
                    {
                       
                        if(_optimizePanel!=null)
                            _optimizePanel.FillTerminalBounds(_funDefinit.GetTerminalNames());
                    };
            }

            if (_dataPanel != null)
            {
                _dataPanel.DataPredictionLoaded += (x, y) =>
                {
                    if (_predictionPanel == null)
                    {
                        _predictionPanel = new PreditionPanel();
                        loadGPPanelInMainWindow(this, _predictionPanel, "Prediction");
                    }
                    if (_runPanel != null)
                    {
                        _predictionPanel.FillPredictionData(_dataPanel.Testing);
                    }

                };
            }


					
		}


        
		/// <summary>
		/// Load page in to GP Main Window. GP Model consis of several specific panels
		/// </summary>
		/// <param name="mainWnd"></param>
		/// <param name="panel"></param>
		/// <param name="TabeItemCaption"></param>
        void loadGPPanelInMainWindow(Form mainWnd, UserControl panel, string TabeItemCaption)
        {
            try
            {
                //ataching the panel in tabPage and page to tabctrl
                TabPage tbfunctionsPage = new TabPage();
                this.tabControl1.Controls.Add(tbfunctionsPage);
                tbfunctionsPage.Controls.Add(panel);

                panel.Anchor = ((System.Windows.Forms.AnchorStyles)
                                ((System.Windows.Forms.AnchorStyles.Top |
                                  System.Windows.Forms.AnchorStyles.Bottom |
                                  System.Windows.Forms.AnchorStyles.Left |
                                  System.Windows.Forms.AnchorStyles.Right)));
                int treeCtrlWidth = 130;
                var size = new Size(mainWnd.Width - treeCtrlWidth - 30, mainWnd.Height - 200);
                panel.Size = size;
                panel.Location = new System.Drawing.Point(0, 0);

                //define tabpage
                tbfunctionsPage.Name = "tabPage" + tabControl1.TabPages.Count.ToString();
                tbfunctionsPage.Padding = new System.Windows.Forms.Padding(2);
                tbfunctionsPage.TabIndex = tabControl1.TabPages.Count;
                tbfunctionsPage.Text = TabeItemCaption;
                tbfunctionsPage.UseVisualStyleBackColor = true;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            
        }

        /// <summary>
        /// About 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbtnInfo_Click(object sender, EventArgs e)
        {
            AboutGPdotNET dlg = new AboutGPdotNET();
            dlg.ShowDialog();
        }

        /// <summary>
        /// Close command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbtnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// running GP algoritam
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void run_GP(object sender, EventArgs e)
        {
            if (_runningEngine > 0)
            {
                MessageBox.Show("Two Engine cannot run at the same time.");
                return;
            }
            if (_resultPanel != null)
            {
                if (_resultPanel.HasPrevSoluton())
                {
                    if (DialogResult.Yes == MessageBox.Show("Would you like to reset previous solution?", Properties.Resources.SR_ApplicationName, MessageBoxButtons.YesNo))
                    {
                        if (_runPanel != null)
                            _runPanel.ResetSolution();
                        if (_resultPanel != null)
                            _resultPanel.ResetSolution();
                        if (_predictionPanel != null)
                            _predictionPanel.ResetSolution();
                    }
                }
            }
            runProgram();
        }
       
        /// <summary>
        /// stop alo 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stopGP(object sender, EventArgs e)
        {
            stopProgram();
        }
       
        /// <summary>
        /// optimize GP model
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void optimizeGP(object sender, EventArgs e)
        {
            if (_runningEngine > 0)
            {
                MessageBox.Show("Two Engine cannot run at the same time.");
                return;
            }
            if (_optimizePanel != null)
            {
                if (_optimizePanel.HasPrevSoluton())
                {
                    if (DialogResult.Yes == MessageBox.Show("Would you like to reset previous solution?",Properties.Resources.SR_ApplicationName,MessageBoxButtons.YesNo))
                    {
                        if (_optimizePanel != null)
                            _optimizePanel.ResetSolution();
                    }
                }
            }
            optimizeProgram();
        }

      
    }
}