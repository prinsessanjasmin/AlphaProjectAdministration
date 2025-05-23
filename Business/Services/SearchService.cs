﻿using Business.Interfaces;
using Business.Models;
using Data.Contexts;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Business.Services;


public class SearchService(IDbContextFactory<DataContext> contextFactory) : ISearchService
{
    //HAd lots of help from botg ChatGPT and Claude AI with this service. 
    private readonly IDbContextFactory<DataContext> _contextFactory = contextFactory;

    public async Task<SearchResult> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return new SearchResult(); 
        }

        query = query.ToLower().Trim();

        var employeeTask = SearchEmployeesAsync(query, cancellationToken);
        var projectTask = SearchProjectsAsync(query, cancellationToken);
        var clientTask = SearchClientsAsync(query, cancellationToken);


        await Task.WhenAll(employeeTask, projectTask, clientTask);

        return new SearchResult
        {
            Employees = await employeeTask,
            Projects = await projectTask,
            Clients = await clientTask,

        };
    }

    public async Task<List<EmployeeSearchResult>> SearchEmployeesAsync(string query, CancellationToken cancellationToken = default)
    {
        var lowerQuery = query.ToLower();

        using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var result = await context.Users
            .Where(e =>
                (e.FirstName != null && e.FirstName.ToLower().Contains(lowerQuery)) ||
                (e.LastName != null && e.LastName.ToLower().Contains(lowerQuery)) ||
                (e.Email != null && e.Email.ToLower().Contains(lowerQuery)) ||
                (e.Address != null && e.Address.StreetAddress != null && e.Address.StreetAddress.ToLower().Contains(lowerQuery)) ||
                (e.Address != null && e.Address.PostCode != null && e.Address.PostCode.Contains(lowerQuery)) ||
                (e.Address != null && e.Address.City != null && e.Address.City.ToLower().Contains(lowerQuery)) ||
                (e.JobTitle != null && e.JobTitle.ToLower().Contains(lowerQuery))
            )
                .Select(e => new EmployeeSearchResult
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                PhoneNumber = e.PhoneNumber,
                Address = ($"{e.Address.StreetAddress} {e.Address.PostCode} {e.Address.City}"),
                JobTitle = e.JobTitle
            }).ToListAsync(cancellationToken);

        return result; 
    }

    public async Task<List<ProjectSearchResult>> SearchProjectsAsync(string query, CancellationToken cancellationToken = default)
    {
        using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var result = await context.Projects
            .Where(p => p.ProjectName.ToLower().Contains(query) ||
                (p.Description != null && p.Description.ToLower().Contains(query))  ||
                p.Client.ClientName.ToLower().Contains(query))
            .Select(p => new ProjectSearchResult
            {
                Id = p.ProjectId,
                ProjectName = p.ProjectName,
                ProjectDescription = p.Description,
                ClientName = p.Client.ClientName
            }).ToListAsync(cancellationToken);

        return result;
    }

    public async Task<List<ClientSearchResult>> SearchClientsAsync(string query, CancellationToken cancellationToken = default)
    {
        using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var result = await context.Clients
            .Where(c => c.ClientName.ToLower().Contains(query))
            .Select(c => new ClientSearchResult
            {
                Id = c.Id,
                ClientName = c.ClientName
            }).ToListAsync(cancellationToken);

        return result;
    }
}
