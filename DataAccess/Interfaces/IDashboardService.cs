using System;
using StoreManagement.ViewModels;

namespace StoreManagement.DataAccess.Interfaces;

public interface IDashboardService
{
      Task<DashboardStatsVM> GetDashboardStatsAsync();
}
