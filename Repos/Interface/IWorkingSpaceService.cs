﻿using Quik_BookingApp.BOs.Request;
using Quik_BookingApp.BOs.Response;
using Quik_BookingApp.Helper;


namespace Quik_BookingApp.Repos.Interface
{
    public interface IWorkingSpaceService
    {
        Task<List<WorkingSpaceRequestModel>> GetAll();
        Task<WorkingSpaceResponseAmenities> GetBySpaceId(string spaceId);
        Task<APIResponse> CreateWS(WorkingSpaceRequestModel ws);

    }
}
