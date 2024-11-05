﻿using dotInstrukcijeBackend.DataTransferObjects;
using dotInstrukcijeBackend.Interfaces.RepositoryInterfaces;
using dotInstrukcijeBackend.Interfaces.Service;
using dotInstrukcijeBackend.Models;
using dotInstrukcijeBackend.ServiceResultUtility;
using dotInstrukcijeBackend.ViewModels;

namespace dotInstrukcijeBackend.Services
{
    public class SessionService : ISessionService
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly IStudentRepository _studentRepository;

        public SessionService(ISessionRepository sessionRepository, IStudentRepository studentRepository)
        {
            _sessionRepository = sessionRepository;
            _studentRepository = studentRepository;
        }


        public async Task<ServiceResult> ScheduleSessionAsync(int studentId, ScheduleSessionModel request)
        {
            if (!await _studentRepository.CanScheduleMoreSessionsAsync(studentId))
            {
                return ServiceResult.Failure("Maximum number of scheduled sessions reached.", 400);
            }

            var session = new Session
            {
                StudentId = studentId,
                ProfessorId = request.ProfessorId,
                SubjectId = request.SubjectId,
                DateTime = request.DateTime,
                Status = "Pending"
            };
            await _sessionRepository.AddSessionAysnc(session);
            return ServiceResult.Success();
        }
        
        
        public async Task<ServiceResult<SessionsDTO<SessionWithProfessorDTO>>> GetAllStudentSessionsAsync(int studentId)
        {
            var sessions = await _sessionRepository.GetAllStudentSessionsAsync(studentId);
            return ServiceResult<SessionsDTO<SessionWithProfessorDTO>>.Success(sessions);
        }


        public async Task<ServiceResult<SessionsDTO<SessionWithStudentDTO>>> GetAllProfessorSessionsAsync(int professorId)
        {
            var sessions = await _sessionRepository.GetAllSessionsForProfessorAsync(professorId);
            return ServiceResult<SessionsDTO<SessionWithStudentDTO>>.Success(sessions);
        }
        
        
        public async Task<ServiceResult> ManageSessionRequestAsync(int professorId, ManageSessionRequestModel request)
        {
            var session = await _sessionRepository.GetSessionByIdAsync(request.SessionId);

            if (session == null)
            {
                return ServiceResult.Failure("Session not found.", 404);
            }

            if (session.ProfessorId != professorId)
            {
                return ServiceResult.Failure("Unauthorized professor.", 403);
            }

            if (request.NewStatus != "Confirmed" && request.NewStatus != "Cancelled")
            {
                return ServiceResult.Failure("Invalid action type.", 400);
            }

            if (request.NewStatus == "Confirmed" && session.Status != "Pending")
            {
                return ServiceResult.Failure("Only pending sessions can be accepted.", 400);
            }

            await _sessionRepository.ManageSessionRequestAsync(session.Id, request.NewStatus);
            return ServiceResult.Success();
        }
    }
}