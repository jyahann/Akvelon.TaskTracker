﻿using Akvelon.TaskTracker.Application.Dtos;
using Akvelon.TaskTracker.Application.Mappers;
using Akvelon.TaskTracker.Data.Helpers;
using Akvelon.TaskTracker.Data.Models;
using Akvelon.TaskTracker.Data.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Akvelon.TaskTracker.Application.Services
{
    public class ProjectsService
    {
        public const string projectNotFoundMsg = "There is no projects with given id";
        private ProjectsRepository _projectsRepository;
        private ProjectMapper _projectMapper = new ProjectMapper();

        public ProjectsService(ProjectsRepository projectsRepository)
        {
            _projectsRepository = projectsRepository;
        }

        public ProjectDto Get(int id)
        {
            var project = _projectsRepository.First(id);
            // null check
            if (project is null)
            {
                throw new KeyNotFoundException(projectNotFoundMsg);
            }

            return _projectMapper.GetProjectDto(_projectsRepository.First(id));
        }

        // check project by id
        public bool Any(int projectId)
        {
            return _projectsRepository.GetAll().Any(p => p.Id == projectId);
        }

        public ProjectDto[] GetAll()
        {
            return _projectsRepository.GetAll().Select(p => _projectMapper.GetProjectDto(p)).ToArray();
        }

        // when project was started afterwards than input date
        public ProjectDto[] GetStartedAt(DateTime startedAt)
        {
            return _projectsRepository.GetAll().Where(p => p.StartedAt >= startedAt).Select(p => _projectMapper.GetProjectDto(p)).ToArray();
        }

        // when project was ended before than input date
        public ProjectDto[] GetEndedAt(DateTime endedAt)
        {
            return _projectsRepository.GetAll().Where(p => p.EndedAt != null && p.EndedAt <= endedAt).Select(p => _projectMapper.GetProjectDto(p)).ToArray();
        }

        // where project's name contains input name
        public ProjectDto[] GetFilteredByName(string name)
        {
            return _projectsRepository.GetAll().Where(p => p.Name.Contains(name)).Select(p => _projectMapper.GetProjectDto(p)).ToArray();
        }

        // where project was started before than input date and ended before than input date
        public ProjectDto[] GetDateRange(DateTime startedAt, DateTime endedAt)
        {
            return _projectsRepository.GetAll().Where(p => (p.EndedAt != null && p.EndedAt >= endedAt) && p.StartedAt >= startedAt).Select(p => _projectMapper.GetProjectDto(p)).ToArray();
        }

        public async System.Threading.Tasks.Task Create(ProjectDto project)
        {
            // if user not entered started at date
            if (project.StartedAt is null)
            {
                project.StartedAt = DateTime.Now;
            }
            // creating new project
            _projectsRepository.Create(_projectMapper.GetProject(project));
            await _projectsRepository.SaveChanges();
        }

        public async System.Threading.Tasks.Task Delete(int projectId)
        {
            // id check
            if (!Any(projectId))
            {
                throw new KeyNotFoundException(projectNotFoundMsg);
            }
            // deleting project
            _projectsRepository.Delete(projectId);
            await _projectsRepository.SaveChanges();
        }

        public async System.Threading.Tasks.Task Update(ProjectDto project)
        {
            // id check
            if (!Any(project.Id))
            {
                throw new KeyNotFoundException(projectNotFoundMsg);
            }
            // updating project
            _projectsRepository.Update(_projectMapper.GetProject(project));
            await _projectsRepository.SaveChanges();
        }
    }
}
