using System;

namespace Portfolio_WPF_App.ViewModels.Interfaces
{
    interface IDataView
    {
        event EventHandler ReloadDataView;
        event EventHandler<ListArguments> SaveData;
        event EventHandler RequestLogData;
        event EventHandler RequestData;
        event EventHandler<ListArguments> RequestLogLevelFilteredData;
        event EventHandler<DateTime> RequestDateTimeLogFilteredData;
        event EventHandler<DateTime> RequestDateTimeFilteredData;
        void LogData(ListArguments data);
        void Data(ListArguments data);
    }
}
