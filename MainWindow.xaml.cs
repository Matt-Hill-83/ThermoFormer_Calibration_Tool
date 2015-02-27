using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using System.Windows.Markup;

namespace Thermoformer {
	public partial class MainWindow : Window {

		#region --------------------------------------------globals------------------------
		CalibrationGrid dataSource = new CalibrationGrid();

		//Init array of objects where each object stores the data for the parameter to be calibrated
		CalibrationGrid ClassForDataGrid = new CalibrationGrid();
		//define timer
		private static System.Windows.Threading.DispatcherTimer eventTimer = new System.Windows.Threading.DispatcherTimer();

		DockPanel DPForAllBlocksOfCalibrationControls = new DockPanel();
		int myRowHeight = 10;
		int controlHeight = 20;
		int calibrationRowWidth = 168;
		int calibrationRowHeight = 26;
		Thickness calibrationGridPositionOffset = new Thickness();
		int verticalSpacingBetweenCalibrationGrids = 2;
		int delayTime = 100;
		int columnHeaderHeight = 40;

		public class XYPoint {
			public float X;
			public float Y;
		}

		#endregion

		public MainWindow() {
			//CalibrationGrid dataSource = new CalibrationGrid();
			//DataContext = this.dataSource;
			DataContext = this.ClassForDataGrid;
			InitializeComponent();
			InitGlobals();
			this.Loaded += AfterWindowLoadsExecuteInitCode;
		}

		//Calculation functions
		public void AddNewCalibrationRowToDataObject() {
			foreach (DataTableRow thisDataTableRow in ClassForDataGrid.ListOfDataTableRows) {
				//In the data structure, look in each calibration grid, in each data row, at the Add Calibration Row checkbox.  If the checkbox is checked, add a new calibration row to the data object 
				int i = 0;
				foreach (CalibrationRow calibrationRow in thisDataTableRow.ListOfCalibrationRows) {
					if (calibrationRow.AddCheckBox.IsChecked == true) {
						//Add new calibration row in data object
						CalibrationRow myCalibrationRow = new CalibrationRow();
						myCalibrationRow.TargetValue = 0;
						thisDataTableRow.ListOfCalibrationRows.Insert(i, myCalibrationRow);
						break;
					}
					i += 1;
				}
			}
		}
		public void DeleteCalibrationRowFromDataObject() {
			foreach (DataTableRow thisDataTableRow in ClassForDataGrid.ListOfDataTableRows) {
				//In the data structure, look in each calibration grid, in each data row, at the Delete Calibration Row checkbox.  If the checkbox is checked, delete a calibration row from the data object 
				int i = 0;
				//Don't allow the last button to be removed.
				bool moreThanOneButtonRemaining = thisDataTableRow.ListOfCalibrationRows.Count > 1;
				foreach (CalibrationRow calibrationRow in thisDataTableRow.ListOfCalibrationRows) {
					if (calibrationRow.DeleteCheckBox.IsChecked == true && moreThanOneButtonRemaining) {
						//Delete corresponding calibration row from data object
						thisDataTableRow.ListOfCalibrationRows.RemoveAt(i);
						break;
					}
					i += 1;
				}
			}
		}
		public void InitGlobals() {
			//Locate calibration grids so that theyoverlay the main grid correctly
			calibrationGridPositionOffset = new Thickness(-calibrationRowWidth - 0, columnHeaderHeight + 1, 0, 0);
		}
		public void StoreFrozenMeasurement(CheckBox clickedCheckBox) {
			//Find the calibration row in which the clicked checkbox resides and store the current measured value in the frozen measured value property
			//Look at each DataTableRow
			foreach (DataTableRow thisDataTableRow in ClassForDataGrid.ListOfDataTableRows) {
				//In the data structure, look in each calibration grid, in each data row, at the RunPause checkbox.  If the checkbox matches the clickedCheckBox, store the corresponding acquired value in FrozenMeasurement
				foreach (CalibrationRow calibrationRow in thisDataTableRow.ListOfCalibrationRows) {
					if (calibrationRow.RunPauseCheckBox == clickedCheckBox) {
						calibrationRow.FrozenMeasurement = thisDataTableRow.AcquiredValue;
					}
				}
			}
		}
		public void CalculateNewConstants(CheckBox clickedCheckBox) {
			//Calculate NewScale and NewOffset
			//Scale and offset constants are calculated by using the formula: Y = aX + b
			//Assume the X values are the measured values and the Y values are the cooresponding true calibrated values (target values)
			//Use the calculations below to create a new a and b

			//Create the inupts to the calculator by creating X,Y pairs for each of the points measured
			List<XYPoint> inputPoints = new List<XYPoint>();

			//Find the calibration row in which the clicked checkbox resides and store the current measured value in the frozen measured value property
			//Look at each DataTableRow
			DataTableRow dataTableRowWhereMeasureValueWasFrozen = new DataTableRow("test", 0,
			0, 0, 0, 3, 1, 1); //todo: I need to create a constructor that takes 0 elements
			foreach (DataTableRow dataTableRow in ClassForDataGrid.ListOfDataTableRows) {
				//In the data structure, look in each calibration grid, in each data row, at the RunPause checkbox.  If the checkbox matches the clickedCheckBox, break, and perform some operations on the DataTableRow
				foreach (CalibrationRow calibrationRow in dataTableRow.ListOfCalibrationRows) {
					if (calibrationRow.RunPauseCheckBox == clickedCheckBox) {
						dataTableRowWhereMeasureValueWasFrozen = dataTableRow;
						break;//When correct dataTableRow is identified, stop searching
					}
				}
			}

			//For each of the CalibrationRows in the GridRow that contains the checked box, 
			foreach (CalibrationRow calibrationRow in dataTableRowWhereMeasureValueWasFrozen.ListOfCalibrationRows) {
				XYPoint myXYPoint = new XYPoint();
				myXYPoint.X = calibrationRow.FrozenMeasurement;
				myXYPoint.Y = calibrationRow.TargetValue;
				inputPoints.Add(myXYPoint);
			}

			//Calculate new a and b values using linear regression.
			//These are the NewScale and NewOffset
			int numPoints = inputPoints.Count;
			double meanX = inputPoints.Average(point => point.X);
			double meanY = inputPoints.Average(point => point.Y);

			double sumXSquared = inputPoints.Sum(point => point.X * point.X);
			double sumXY = inputPoints.Sum(point => point.X * point.Y);

			double a = (sumXY / numPoints - meanX * meanY) / (sumXSquared / numPoints - meanX * meanX);
			double b = (a * meanX - meanY);

			dataTableRowWhereMeasureValueWasFrozen.NewScale = (float)a;
			dataTableRowWhereMeasureValueWasFrozen.NewOffset = (float)b;

			//return inputPoints.Select(point => new XYPoint() { X = point.X, Y = a1 * point.X - b1 }).ToList();
		}

		//GUI design functions
		public void InitGui() {
			RedrawGui();
			//locate grid calibration controls with respect to datagrid
			dockPanel_MainDockpanel.Margin = calibrationGridPositionOffset;
			boundDataGrid.ColumnHeaderHeight = columnHeaderHeight;
			//boundDataGrid.ColumnHeaderStyle.Setters.Add(Background.Opacity(3));
			//just a test
			//CreateCheckBoxesTest();

		}
		public void SetUpOuterDockPanels() {
			DockPanel.SetDock(DPForAllBlocksOfCalibrationControls, System.Windows.Controls.Dock.Left);
			DPForAllBlocksOfCalibrationControls.Margin = new Thickness(0, 0, 0, 0);
			dockPanel_MainDockpanel.Children.Add(DPForAllBlocksOfCalibrationControls);
		}
		public void SetUpCalibrationDataBlockDockPanels() {
			//For each Calibration Data Block, create a set of Controls to represent its elements in the GUI
			int i = -1;
			foreach (DataTableRow thisDataTableRow in ClassForDataGrid.ListOfDataTableRows) {
				i += 1;

				// create a DockPanel Control that contains each Block of Calibration Controls
				DockPanel BlockOfCalibrationControls = CreateBlockOfCalibrationControls(i);
				BlockOfCalibrationControls.Margin = new Thickness(0, 0, 0, verticalSpacingBetweenCalibrationGrids);
				DockPanel.SetDock(BlockOfCalibrationControls, System.Windows.Controls.Dock.Top);

				//Nest child Controls inside parents
				DPForAllBlocksOfCalibrationControls.Children.Add(BlockOfCalibrationControls);
			}
		}
		public void RedrawGui() {
			RemoveExistingRowsOfCalibrationControlsFromGui();
			SetUpOuterDockPanels();
			SetUpCalibrationDataBlockDockPanels();
			DockPanel DPToFillSpaceToTheRight = new DockPanel();
			dockPanel_MainDockpanel.Children.Add(DPToFillSpaceToTheRight);
			ResizeDataGridRowHeight();
		}

		//Create GUI elements
		public DockPanel CreateBlockOfCalibrationControls(int dataRowNumber) {

			DockPanel DPForBlockOfCalibrationControls = new DockPanel();
			//create color for each dock panel
			DPForBlockOfCalibrationControls.Background = Brushes.Transparent;
			DockPanel.SetDock(DPForBlockOfCalibrationControls, System.Windows.Controls.Dock.Left);
			DPForBlockOfCalibrationControls.Margin = new Thickness(0, 0, 0, 0);

			//Count how many calibration sample rows there are in the data object and create a matching number of calibration rows to be displayed as if they are contained in each main row
			int numberOfInnerDockPanels = ClassForDataGrid.ListOfDataTableRows[dataRowNumber].ListOfCalibrationRows.Count;
			for (int calibrationRowNumber = 0; calibrationRowNumber < numberOfInnerDockPanels; calibrationRowNumber++) {
				//create inner dockpanel to contain 2 text boxes and 3 buttons
				DockPanel dockPanelForCalibrationRow = new DockPanel();
				dockPanelForCalibrationRow.Background = Brushes.Transparent;
				dockPanelForCalibrationRow.Margin = new Thickness(1, 1, 1, 1);
				DockPanel.SetDock(dockPanelForCalibrationRow, System.Windows.Controls.Dock.Top);

				CreateTextBoxes(dockPanelForCalibrationRow, dataRowNumber, calibrationRowNumber);
				CreateCheckBoxes(dockPanelForCalibrationRow, dataRowNumber, calibrationRowNumber);
				//Add DockPanel for each calibration row to the larger dockpanel
				DPForBlockOfCalibrationControls.Children.Add(dockPanelForCalibrationRow);
			}
			return DPForBlockOfCalibrationControls;
		}
		public void CreateTextBoxes(DockPanel dockPanelForCalibrationRow, int dataRowNumber, int calibrationRowNumber) {
			//Create textboxes
			TextBox referenceToTargetValueTextBox = AddTextBox(dockPanelForCalibrationRow, new SolidColorBrush(Colors.Red), dataRowNumber, calibrationRowNumber);//for TargetValue
			TextBox referenceToMeasuredValueTextBox = AddTextBox(dockPanelForCalibrationRow, new SolidColorBrush(Colors.Orange), dataRowNumber, calibrationRowNumber);//For measured value

			//Bind textboxes to data in data structure
			//Bind the Acquired Value TextBox to its corresponding data item. This item is a level up fromteh calibration row, since all calibration rows have the same acquired value, which comes from the data acquisition driver
			bool isFrozen = ClassForDataGrid.ListOfDataTableRows[dataRowNumber].ListOfCalibrationRows[calibrationRowNumber].RunPauseCheckBoxState == true;

			if (isFrozen) {
				Binding acquiredValueBinding = new Binding("FrozenMeasurement");
				acquiredValueBinding.Source = ClassForDataGrid.ListOfDataTableRows[dataRowNumber].ListOfCalibrationRows[calibrationRowNumber];
				acquiredValueBinding.Mode = BindingMode.TwoWay;
				referenceToMeasuredValueTextBox.SetBinding(TextBox.TextProperty, acquiredValueBinding);
			}
			else {
				Binding acquiredValueBinding = new Binding("AcquiredValue");
				acquiredValueBinding.Source = ClassForDataGrid.ListOfDataTableRows;
				acquiredValueBinding.Mode = BindingMode.TwoWay;
				referenceToMeasuredValueTextBox.SetBinding(TextBox.TextProperty, acquiredValueBinding);
			}

			//Bind the Target Value TextBox to its corresponding data item in the calibration row
			Binding targetValueBinding = new Binding("TargetValue");
			targetValueBinding.Source = ClassForDataGrid.ListOfDataTableRows[dataRowNumber].ListOfCalibrationRows[calibrationRowNumber];
			targetValueBinding.Mode = BindingMode.TwoWay;
			referenceToTargetValueTextBox.SetBinding(TextBox.TextProperty, targetValueBinding);
		}
		public TextBox AddTextBox(DockPanel aDockPanel, SolidColorBrush textBoxBorderColor, int rowNumber, int calibrationRowNumber) {

			//Define border
			Border textBoxBorder = new Border();
			SolidColorBrush mySolidColorBrush = textBoxBorderColor;
			textBoxBorder.BorderBrush = mySolidColorBrush;
			textBoxBorder.BorderThickness = new Thickness(1);
			textBoxBorder.CornerRadius = new CornerRadius(2);
			textBoxBorder.Height = controlHeight;
			DockPanel.SetDock(textBoxBorder, System.Windows.Controls.Dock.Left);

			//Define Textbox
			textBoxBorder.Margin = new Thickness(1, 0, 2, 0);
			//Dock Textbox Left inside Border ??
			TextBox newTextBox = new TextBox();
			newTextBox.Width = 40;
			//newTextBox.Height = 0;//let the Border specify the height
			newTextBox.FontSize = 11;
			newTextBox.BorderThickness = new Thickness(1);
			DockPanel.SetDock(newTextBox, System.Windows.Controls.Dock.Left);
			newTextBox.Margin = new Thickness(0, 0, 0, 0);
			newTextBox.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
			newTextBox.VerticalContentAlignment = System.Windows.VerticalAlignment.Top;
			newTextBox.VerticalAlignment = System.Windows.VerticalAlignment.Center;

			SolidColorBrush textBoxSolidColorBrush = new SolidColorBrush(Colors.Red);
			textBoxSolidColorBrush.Opacity = 0;
			newTextBox.BorderBrush = textBoxSolidColorBrush;

			//Place TextBox in Border
			textBoxBorder.Child = newTextBox;
			//Place Border in Dockpanel
			aDockPanel.Children.Add(textBoxBorder);
			//Return the reference to the TextBox so that data binding can be performed on it
			return newTextBox;
		}
		public void CreateCheckBoxes(DockPanel dockPanelForCalibrationRow, int dataRowNumber, int calibrationRowNumber) {
			#region -------------------------------Run/Pause Checkbox
			//Redraw the boxes in the correct checked/unchecked state
			CheckBox checkBoxRun_Pause = new CheckBox();
			Image imageCheckBoxRun_Pause = new Image();
			imageCheckBoxRun_Pause.Width = controlHeight;
			imageCheckBoxRun_Pause.Height = controlHeight;
			imageCheckBoxRun_Pause.Margin = new Thickness(-18, 0, 4, 0); //reposition image so that it overlays checkbox
			//Select correct image based on run or pause state
			imageCheckBoxRun_Pause.Source = new BitmapImage(new Uri("Images/pause_001.jpg", UriKind.Relative));

			bool x;

			x = ClassForDataGrid.ListOfDataTableRows[dataRowNumber].ListOfCalibrationRows[calibrationRowNumber].RunPauseCheckBoxState;

			if (x) {
				imageCheckBoxRun_Pause.Source = new BitmapImage(new Uri("Images/play_001.jpg", UriKind.Relative));
			}

			checkBoxRun_Pause.Margin = new Thickness(2, 0, 0, 0);
			checkBoxRun_Pause.Content = imageCheckBoxRun_Pause;

			//This links events from my buttons to a function that refreshes the GUI so that when certain buttons are pressed on the GUI, the GUI is refreshed accordingly
			checkBoxRun_Pause.Click += new RoutedEventHandler(WhenPlayPauseButtonClickedFreezeLiveData);

			Binding runPauseCheckBoxBinding = new Binding("RunPauseCheckBoxState");
			runPauseCheckBoxBinding.Source = ClassForDataGrid.ListOfDataTableRows[dataRowNumber].ListOfCalibrationRows[calibrationRowNumber];
			runPauseCheckBoxBinding.Mode = BindingMode.TwoWay;
			checkBoxRun_Pause.SetBinding(CheckBox.IsCheckedProperty, runPauseCheckBoxBinding);

			//Save the reference to the checkbox in the data object
			ClassForDataGrid.ListOfDataTableRows[dataRowNumber].ListOfCalibrationRows[calibrationRowNumber].RunPauseCheckBox = checkBoxRun_Pause;

			//Add checkbox to DockPanel
			dockPanelForCalibrationRow.Children.Add(checkBoxRun_Pause);
			#endregion
			#region -------------------------------Add Checkbox
			//Always redraw the boxes unchecked
			CheckBox checkBoxAddCalibrationRow = new CheckBox();
			Image imageCheckBox = new Image();
			imageCheckBox.Width = controlHeight;
			imageCheckBox.Height = controlHeight;
			imageCheckBox.Margin = new Thickness(-18, 0, 4, 0); //reposition image so that it overlays checkbox
			imageCheckBox.Source = new BitmapImage(new Uri("Images/add_001.jpg", UriKind.Relative));
			checkBoxAddCalibrationRow.Margin = new Thickness(2, 0, 0, 0);
			checkBoxAddCalibrationRow.Content = imageCheckBox;

			//This links events from my buttons to a function that refreshes the GUI so that when certain buttons are pressed on the GUI, the GUI is refreshed accordingly
			checkBoxAddCalibrationRow.Click += new RoutedEventHandler(WhenAddButtonClickedAddCalibrationRow);

			//Save the reference to the checkbox in the data object
			ClassForDataGrid.ListOfDataTableRows[dataRowNumber].ListOfCalibrationRows[calibrationRowNumber].AddCheckBox = checkBoxAddCalibrationRow;
			//Add checkbox to DockPanel
			dockPanelForCalibrationRow.Children.Add(checkBoxAddCalibrationRow);
			#endregion
			#region -------------------------------Delete Checkbox
			//Always redraw the boxes unchecked
			CheckBox checkBoxDeleteCalibrationRow = new CheckBox();
			Image imageCheckBoxDelete = new Image();
			imageCheckBoxDelete.Width = controlHeight;
			imageCheckBoxDelete.Height = controlHeight;
			imageCheckBoxDelete.Margin = new Thickness(-18, 0, 4, 0); //reposition image so that it overlays checkbox
			imageCheckBoxDelete.Source = new BitmapImage(new Uri("Images/x_001.jpg", UriKind.Relative));
			checkBoxDeleteCalibrationRow.Margin = new Thickness(2, 0, 0, 0);
			checkBoxDeleteCalibrationRow.Content = imageCheckBoxDelete;

			//This links events from my buttons to a function that refreshes the GUI so that when certain buttons are pressed on the GUI, the GUI is refreshed accordingly
			checkBoxDeleteCalibrationRow.Click += new RoutedEventHandler(WhenDeleteButtonClickedDeleteCalibrationRow);

			//Save the reference to the checkbox in the data object
			ClassForDataGrid.ListOfDataTableRows[dataRowNumber].ListOfCalibrationRows[calibrationRowNumber].DeleteCheckBox = checkBoxDeleteCalibrationRow;

			//Add checkbox to DockPanel
			dockPanelForCalibrationRow.Children.Add(checkBoxDeleteCalibrationRow);
			#endregion
		}

		//Adjust GUI elements
		private void ResizeDataGridRowHeight() {
			int a = boundDataGrid.Items.Count;
			for (int i = 0; i < a; i++) {
				int numCalibrationRows = ClassForDataGrid.ListOfDataTableRows[i].ListOfCalibrationRows.Count;
				myRowHeight = (numCalibrationRows * calibrationRowHeight) - (numCalibrationRows - 0) * 4 + 2;
				DataGridRow row = boundDataGrid.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
				row = boundDataGrid.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
				row.Height = myRowHeight;
				//disable the row so it is read only
				row.IsEnabled = false;
			}
		}
		public void HideNonRelevantDataGridRows() {
			int numColumns = boundDataGrid.Columns.Count;
			int numColumnsToDisplay = 5;
			for (int i = numColumnsToDisplay; i < numColumns; i++) {
				boundDataGrid.Columns[i].Visibility = Visibility.Hidden;
			}
			//add blank dummy column to house caligration grid
			DataGridTextColumn blankColumnforCalibrationGridPlacement = new DataGridTextColumn();
			blankColumnforCalibrationGridPlacement.Header = "target actual run/stop add delete";
			blankColumnforCalibrationGridPlacement.Width = calibrationRowWidth;
			//doesn't work
			//blankColumnforCalibrationGridPlacement.FontSize = 20;

			boundDataGrid.Columns.Add(blankColumnforCalibrationGridPlacement);
		}
		public void RemoveExistingRowsOfCalibrationControlsFromGui() {
			//remove old rows of calibration controls from GUI
			DPForAllBlocksOfCalibrationControls.Children.Clear();
			dockPanel_MainDockpanel.Children.Remove(DPForAllBlocksOfCalibrationControls);
		}

		//Timer functions
		public void InitializeAndStartTimer() {
			eventTimer.Tick += CodeToExecuteOnTimerTick;
			eventTimer.Interval = new TimeSpan(0, 0, 0, 0, delayTime);
			eventTimer.Start();
		}
		public void CodeToExecuteOnTimerTick(object sender, EventArgs e) {

			foreach (DataTableRow dataTableRow in ClassForDataGrid.ListOfDataTableRows) {
				Random random = new Random();
				float r = random.Next(190, 210);
				dataTableRow.AcquiredValue = r;
			}
		}

		//Events
		public void AfterWindowLoadsExecuteInitCode(object sender, RoutedEventArgs e) {
			InitGui();
			HideNonRelevantDataGridRows();
			InitializeAndStartTimer();
		}
		public void WhenAddButtonClickedAddCalibrationRow(object sender, RoutedEventArgs e) {
			AddNewCalibrationRowToDataObject();
			RedrawGui();
			ResizeDataGridRowHeight();
		}
		public void WhenDeleteButtonClickedDeleteCalibrationRow(object sender, RoutedEventArgs e) {
			DeleteCalibrationRowFromDataObject();
			RedrawGui();
		}
		public void WhenPlayPauseButtonClickedFreezeLiveData(object sender, RoutedEventArgs e) {
			//CheckBox newBox = e.Source;
			//todo: how do I get access to the CheckBox trigger for the event and its parents Neville
			//why is my source null in the code below?
			//TextBox source = e.OriginalSource as TextBox;
			CheckBox clickedCheckBox = e.Source as CheckBox;
			//if (source != null) {
			//	MessageBox.Show("You pressed " + source.Name, Title);
			//}

			StoreFrozenMeasurement(clickedCheckBox);
			CalculateNewConstants(clickedCheckBox);
			RedrawGui();
		}
		}
	}
