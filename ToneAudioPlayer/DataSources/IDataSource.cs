using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ToneAudioPlayer.Services;

namespace ToneAudioPlayer.DataSources;

public interface IDataSource
{
    public string Id { get; }
    public Task<List<DataSourceItem>> SearchAsync(string q);
    
    public Task<DataSourceItem?> GetItemByIdAsync(string id);
    // public Task<List<DataSourceItem>> LookupItems(string id);
    
    public void HandleAction(DataSourceItem item, DataSourceAction action, object? context=null);

}