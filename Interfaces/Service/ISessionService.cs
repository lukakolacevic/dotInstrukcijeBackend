﻿using dotInstrukcijeBackend.DataTransferObjects;
using dotInstrukcijeBackend.Models;
using dotInstrukcijeBackend.ServiceResultUtility;
using dotInstrukcijeBackend.ViewModels;

namespace dotInstrukcijeBackend.Interfaces.Service
{
    public interface ISessionService
    {
        Task<ServiceResult> ScheduleSessionAsync(int studentId, ScheduleSessionModel request);
        Task<ServiceResult<SessionsDTO<SessionWithUserDTO>>> GetAllStudentSessionsAsync(int studentId);
        Task<ServiceResult<SessionsDTO<SessionWithUserDTO>>> GetAllInstructorSessionsAsync(int instructorId);
        Task<ServiceResult> ManageSessionRequestAsync(int professorId, ManageSessionRequestModel request);
    }
}
