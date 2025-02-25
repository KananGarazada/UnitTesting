﻿using AutoMapper;
using EmployeeManagement.Business;
using EmployeeManagement.Controllers;
using EmployeeManagement.DataAccess.Entities;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace EmployeeManagement.Test
{
    public class InternalEmployeeControllerTests
    {
        private readonly InternalEmployeesController _internalEmployeesController;
        private readonly InternalEmployee _firstEmployee;

        public InternalEmployeeControllerTests()
        {
            _firstEmployee = new ("Kanan", "Garazada", 2, 3000, false, 2)
            {
                Id = Guid.Parse("bfdd0acd-d314-48d5-a7ad-0e94dfdd9155"),
                SuggestedBonus = 400
            };

            var employeeServiceMock = new Mock<IEmployeeService>();
            employeeServiceMock
                .Setup(m => m.FetchInternalEmployeesAsync())
                .ReturnsAsync(new List<InternalEmployee>(){
                    _firstEmployee,
                    new InternalEmployee("Kanan", "Garazada", 3, 3000, false,2),
                    new InternalEmployee("Kanan", "Garazada", 3, 3000, false,2)
                });

            //var mapperMock = new Mock<IMapper>();
            //mapperMock.Setup(m =>
            //     m.Map<InternalEmployee, Models.InternalEmployeeDto>
            //     (It.IsAny<InternalEmployee>()))
            //     .Returns(new Models.InternalEmployeeDto());

            var mapperConfiguration = new MapperConfiguration(
               cfg => cfg.AddProfile<MapperProfiles.EmployeeProfile>());
            var mapper = new Mapper(mapperConfiguration);

            _internalEmployeesController = new InternalEmployeesController(employeeServiceMock.Object, mapper);
        }

        [Fact]
        public async Task GetInternalEmployee_GetAction_MustReturnOkObjectResult()
        {
            //Arrange

            //Act 
            var result = await _internalEmployeesController.GetInternalEmployees();

            //Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Models.InternalEmployeeDto>>>(result);  
            Assert.IsType<OkObjectResult>(actionResult.Result);
        }

        [Fact]
        public async Task GetInternalEmployees_GetAction_MustReturnIEnumerableOfInternalEmployeesDtoAsModelType()
        {
            //Arrange

            //Act
            var result = await _internalEmployeesController.GetInternalEmployees();

            //Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Models.InternalEmployeeDto>>>(result);
            
            Assert.IsAssignableFrom<IEnumerable<InternalEmployeeDto>>(
                ((OkObjectResult)actionResult.Result).Value);
        }

        [Fact]
        public async Task GetInternalEmployees_GetAction_MustReturnNumberOfInputtedInternalEmployees()
        {
            //Arrange

            //Act
            var result = await _internalEmployeesController.GetInternalEmployees();

            //Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<InternalEmployeeDto>>>(result);

            Assert.Equal(3,
                ((IEnumerable<InternalEmployeeDto>)
                ((OkObjectResult)actionResult.Result).Value).Count());
        }

        [Fact]
        public async Task GetInternalEmployees_GetAction_ReturnsOkObjectResultWithCorrectAmountOfInternalEmployees()
        {
            //Arrange

            //Act
            var result = await _internalEmployeesController.GetInternalEmployees();

            //Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<InternalEmployeeDto>>>(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<InternalEmployeeDto>>
                (okObjectResult.Value);
            Assert.Equal(3,dtos.Count());
            var firstEmployee = dtos.First();
            Assert.Equal(_firstEmployee.Id, firstEmployee.Id);
            Assert.Equal(_firstEmployee.FirstName, firstEmployee.FirstName);
            Assert.Equal(_firstEmployee.LastName, firstEmployee.LastName);
            Assert.Equal(_firstEmployee.Salary, firstEmployee.Salary);
            Assert.Equal(_firstEmployee.SuggestedBonus, firstEmployee.SuggestedBonus);
            Assert.Equal(_firstEmployee.YearsInService, firstEmployee.YearsInService);
        }
    }
}
