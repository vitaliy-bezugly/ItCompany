﻿using CompanyController.Abstract;
using Domain.Client;
using Domain.Client.Abstract;
using Domain.Company;
using ItCompany.UI.Models;
using Mappers;

namespace CompanyController;

public class DomainController : IController
{
    private readonly IFormLogger _logger;
    private readonly List<Company> _companies;

    private int _companyCount;
    private int _departmentCount;
    private int _usernameCount;
    private int _projectCount;
    public DomainController(IFormLogger logger)
    {
        _logger = logger;
        _companies = new List<Company>();

        _companyCount = 1;
        _departmentCount = 1;
        _usernameCount = 1;
        _projectCount = 1;
    }

    public CompanyViewModel ConfigureClients(int countOfClients, CompanyViewModel company)
    {
        Company? companyToBind = _companies.FirstOrDefault(x => x.Id == company.Id);

        if (companyToBind == null)
            throw new InvalidOperationException();

        for (int i = 0; i < countOfClients; i++)
        {
            var client = new Client(companyToBind, Guid.NewGuid(), $"User #{_usernameCount++}"
                , Random.Shared.Next(100000, 500000));
            OrderProjects(client, 2);
        }

        return companyToBind.ToModel();
    }

    public void StartProcess(CompanyViewModel company, Guid projectId)
    {
        var domain = _companies.FirstOrDefault(x => x.Id == company.Id);
        if (domain == null)
            throw new InvalidOperationException();

        domain.StartWorkOnProject(projectId);
    }

    public CompanyViewModel ConfigureCompany()
    {
        var companyBuilder = new CompanyBuilder();

        companyBuilder.SetCompanyName($"Company: #{_companyCount++}");
        for (int i = 0; i < 5; i++)
        {
            companyBuilder.CreateDepartmentAndAddToCompany($"Department #{_departmentCount++}");
        }

        companyBuilder.AddLogger(_logger);

        var company = companyBuilder.Build();
        _companies.Add(company);
        return company.ToModel();
    }

    public CompanyViewModel? GetCurrentCompanyState(Guid id)
    {
        return _companies.FirstOrDefault(x => x.Id == id)?.ToModel();
    }

    public CompanyViewModel? CreateAndOrderProjects(Guid companyId, int projectsCount)
    {
        var company = _companies.FirstOrDefault(x => x.Id == companyId);

        if (company == null)
            return null;

        foreach (var item in company.GetClients())
        {
            OrderProjects(item, Random.Shared.Next(1, 3));
        }

        return company.ToModel();
    }

    private void OrderProjects(BaseClient client, int projectsCount)
    {
        for (int i = 0; i < projectsCount; i++)
        {
            client.OrderProject(new ClientProject(Guid.NewGuid(), $"Project #{_projectCount++}",
                Random.Shared.Next(1800, 4500), DateOnly.FromDateTime(DateTime.Now.AddDays(Random.Shared.Next(14, 141)))));
        }
    }
}
