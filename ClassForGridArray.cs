//myCalibrationRow.TargetValue = 777;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using DevExpress.Xpf.Editors;
using System.Windows;
using System.Windows.Documents;
using DevExpress.Xpf.Grid;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Thermoformer {

	//This is the class that holds the data structure that contains:
	//1 - The datagrid
	//2 - The fake grid of calibration data rows that overlays the datagrid and appears to be a part of the datagrid
	public class CalibrationGrid : INotifyPropertyChanged {
		DataTableRow currentDataTableRow;
		//An instance of the class can access LissofDataTableRows
		public ObservableCollection<DataTableRow> ListOfDataTableRows { get; set; }//create list of objects as observable
		public event PropertyChangedEventHandler PropertyChanged;
		public DataTableRow CurrentDataTableRow {
			get { return currentDataTableRow; }
			set {
				if (currentDataTableRow == value) return;
				currentDataTableRow = value;
				RaisePropertyChanged("CurrentItem2");
			}
		}
		public CalibrationGrid() {
			ListOfDataTableRows = new ObservableCollection<DataTableRow>();
			ListOfDataTableRows.Add(new DataTableRow("Air Pressure (psi)", 90, 90, 90, 90,2,100,0));
			ListOfDataTableRows.Add(new DataTableRow("Thickness (mm)", 90, 90, 90, 90, 2, 50, 0));
			ListOfDataTableRows.Add(new DataTableRow("TC 1 (C)", 90, 90, 90, 90, 3, 50, 0));
			ListOfDataTableRows.Add(new DataTableRow("TC 2(C)", 90, 90, 90, 90, 3, 50, 0));
			ListOfDataTableRows.Add(new DataTableRow("TC 3 (C)", 90, 90, 90, 90, 3, 50, 0));
			ListOfDataTableRows.Add(new DataTableRow("Velocity (mm/s)", 90, 90, 90, 90, 3, 50, 0));
			ListOfDataTableRows.Add(new DataTableRow("Length (mm)", 90, 90, 90, 90, 3, 50, 0));

			InitItems();
		}
		void InitItems() {
			//Create initial values for items in the calibration grid rows
			int j = 0;
			foreach (DataTableRow row in ListOfDataTableRows) {
				row.CurrentScale = 1001 + j;
				row.CurrentOffset = 100 + j;
				row.NewScale = 200 + j;
				row.NewOffset = 300 + j;
				//Add some dummy calibration rows
				int numCalRows = row.MinNumOfCalibrationRows;

				for (int i = 0; i < numCalRows; i++) {
					CalibrationRow myCalibrationRow = new CalibrationRow();
					myCalibrationRow.TargetValue = 100;//set initial values of 0
					CheckBox myCheckBox = new CheckBox();
					myCalibrationRow.RunPauseCheckBox = myCheckBox;
					myCalibrationRow.RunPauseCheckBox.IsChecked = null;//set initial values of 0
					myCalibrationRow.RunPauseCheckBoxState = false;
					row.ListOfCalibrationRows.Add(myCalibrationRow);
				}
				j += 1;
			}
		}
		void RaisePropertyChanged(string propertyName) {
			if (PropertyChanged == null) return;
			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public class DataTableRow : INotifyPropertyChanged {
		string _parameter;
		float _currentScale;
		float _currentOffset;
		float _newScale;
		float _newOffset;
		int _minNumOfCalibrationRows;
		int _rowHeight;
		float _acquiredValue;
		List<CalibrationRow> _listOfCalibrationRows;

		public string Parameter {
			get { return _parameter; }
			set {
				if (_parameter == value) return;
				_parameter = value;
				RaisePropertyChanged("Parameter");
			}
		}
		public float CurrentScale {
			get { return _currentScale; }
			set {
				if (_currentScale == value) return;
				_currentScale = value;
				RaisePropertyChanged("CurrentScale");
			}
		}
		public float CurrentOffset {
			get { return _currentOffset; }
			set {
				if (_currentOffset == value) return;
				_currentOffset = value;
				RaisePropertyChanged("CurrentOffset");
			}
		}
		public float NewScale {
			get { return _newScale; }
			set {
				if (_newScale == value) return;
				_newScale = value;
				RaisePropertyChanged("NewScale");
			}
		}
		public float NewOffset {
			get { return _newOffset; }
			set {
				if (_newOffset == value) return;
				_newOffset = value;
				RaisePropertyChanged("NewOffset");
			}
		}
		public int MinNumOfCalibrationRows {
			get { return _minNumOfCalibrationRows; }
			set {
				if (_minNumOfCalibrationRows == value) return;
				_minNumOfCalibrationRows = value;
				RaisePropertyChanged("MinNumOfCalibrationRows");
			}
		}
		public int RowHeight {
			get { return _rowHeight; }
			set {
				if (_rowHeight == value) return;
				_rowHeight = value;
				RaisePropertyChanged("RowHeight");
			}
		}
		public float AcquiredValue {
			get { return _acquiredValue; }
			set {
				if (_acquiredValue == value) return;
				_acquiredValue = value;
				//todo: how do I make the text below dynamic instead of static?
				RaisePropertyChanged("AcquiredValue");
			}
		}
		public List<CalibrationRow> ListOfCalibrationRows {
			get { return _listOfCalibrationRows; }
			set {
				if (_listOfCalibrationRows == value) return;
				_listOfCalibrationRows = value;
				RaisePropertyChanged("ListOfCalibrationRows");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		void RaisePropertyChanged(string fieldName) {
			if (PropertyChanged == null) return;
			PropertyChanged(this, new PropertyChangedEventArgs(fieldName));
		}
		public DataTableRow(string parameter, float currentScale,
			float currentOffset, float newScale, float newOffset, int minNumOfCalibrationRows, int rowHeight, float acquiredValue) {
			Parameter = parameter;
			CurrentScale = currentScale;
			CurrentOffset = currentOffset;
			NewScale = newScale;
			NewOffset = newOffset;
			MinNumOfCalibrationRows = minNumOfCalibrationRows;
			ListOfCalibrationRows = new List<CalibrationRow>();
			RowHeight = rowHeight;
			AcquiredValue = acquiredValue;
		}
	}
	public class CalibrationRow : INotifyPropertyChanged {
		//todo: these should all have underscores
		float targetValue;
		float measuredValue;
		float frozenMeasurement;
		bool runPauseCheckBoxState; //Do I need these if I create the Button?
		Button deleteButton;
		CheckBox addCheckBox;//This is a reference to the actual checkbox so you can grab the value in the checkbox.  I could just bind this
		CheckBox deleteCheckBox;//This is a reference to the actual checkbox so you can grab the value in the checkbox.  I could just bind this
		CheckBox runPauseCheckBox;//This is a reference to the actual checkbox so you can grab the value in the checkbox.  I could just bind this

		public float TargetValue {
			get { return targetValue; }
			set {
				if (targetValue == value) return;
				targetValue = value;
				RaisePropertyChanged("TargetValue");
			}
		}
		public float MeasuredValue {
			get { return measuredValue; }
			set {
				if (measuredValue == value) return;
				measuredValue = value;
				RaisePropertyChanged("MeasuredValue");
			}
		}
		public float FrozenMeasurement {
			get { return frozenMeasurement; }
			set {
				if (frozenMeasurement == value) return;
				frozenMeasurement = value;
				RaisePropertyChanged("FrozenMeasurement");
			}
		}
		public bool RunPauseCheckBoxState {
			get { return runPauseCheckBoxState; }
			set {
				if (runPauseCheckBoxState == value) return;
				runPauseCheckBoxState = value;
				RaisePropertyChanged("RunPauseCheckBoxState");
			}
		}
		public Button DeleteButton {
			get { return deleteButton; }
			set {
				if (deleteButton == value) return;
				deleteButton = value;
				RaisePropertyChanged("DeleteButton");
			}
		}
		public CheckBox AddCheckBox {
			get { return addCheckBox; }
			set {
				if (addCheckBox == value) return;
				addCheckBox = value;
				RaisePropertyChanged("AddCheckBox");
			}
		}
		public CheckBox DeleteCheckBox {
			get { return deleteCheckBox; }
			set {
				if (deleteCheckBox == value) return;
				deleteCheckBox = value;
				RaisePropertyChanged("DeleteCheckBox");
			}
		}
		public CheckBox RunPauseCheckBox {
			get { return runPauseCheckBox; }
			set {
				if (runPauseCheckBox == value) return;
				runPauseCheckBox = value;
				RaisePropertyChanged("RunPauseCheckBox");
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		void RaisePropertyChanged(string propertyName) {
			if (PropertyChanged == null) return;
			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
