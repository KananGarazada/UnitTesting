﻿using EmployeeManagement.Business;
using EmployeeManagement.DataAccess.DbContexts;
using EmployeeManagement.DataAccess.Entities;
using EmployeeManagement.DataAccess.Services;
using EmployeeManagement.Services.Test;
using EmployeeManagement.Test.HttpMessageHandlers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace EmployeeManagement.Test
{
    public class TestIsolationApproachesTests
    {
        [Fact]
        public async Task AttendCourseAsync_CourseAttended_SuggestedBonusMustBeCorrectlyBeRecalculated()
        {
            //Arrange
            var connection = new SqliteConnection("Data Source=:memory:");
            connection.Open();

            var optionsBuilder = new DbContextOptionsBuilder<EmployeeDbContext>().UseSqlite(connection);

            var dbContext = new EmployeeDbContext(optionsBuilder.Options);
            dbContext.Database.Migrate();

            var employeeManagementDataRepository = new EmployeeManagementRepository(dbContext);

            var employeeService = new EmployeeService(employeeManagementDataRepository, new EmployeeFactory());

            //get course from dataBase
            var courseToAttend = await employeeManagementDataRepository.
                GetCourseAsync(Guid.Parse("844e14ce-c055-49e9-9610-855669c9859b"));
            //get existing employee
            var internalEmployee = await employeeManagementDataRepository.
                GetInternalEmployeeAsync(Guid.Parse("72f2f5fe-e50c-4966-8420-d50258aefdcb"));

            if(internalEmployee == null || courseToAttend == null)
            {
                throw new XunitException("Arranging the test failed");
            }

            //expected suggested bonus after attending the course
            var expectedSuggestedBonus = internalEmployee.YearsInService
                * (internalEmployee.AttendedCourses.Count + 1) * 100;

            //Act
            await employeeService.AttendCourseAsync(internalEmployee, courseToAttend);

            //Assert
            Assert.Equal(expectedSuggestedBonus, internalEmployee.SuggestedBonus);
        }

        [Fact]
        public async Task PromoteInernalEmployeeAsync_IsEligible_JobLevelMustBeIncreased()
        {
            //Arrange
            var httpClient = new HttpClient(new TestablePromotionEligibilityHandler(true));
            var internalEmployee = new InternalEmployee("Kanan", "Garazada", 3, 3000, false, 1);
            var promotionService = new PromotionService(httpClient, new EmployeeManagementTestDataRepository());

            //Act
            await promotionService.PromoteInternalEmployeeAsync(internalEmployee);

            //Assert
            Assert.Equal(2, internalEmployee.JobLevel);
        }
    }
}
