﻿using EmployeeManagement.ActionFilters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EmployeeManagement.Test
{
    public class CheckShowStatitisticsHeaderTests
    {
        [Fact]
        public void OnActionExecuting_InvokeWithoutShowStatistics_ReturnsBadRequest()
        {
            //Arrange
            var checkShowStatisticsHeaderActionFilter = new CheckShowStatisticsHeader();

            var httpContext = new DefaultHttpContext();

            var actionContext = new ActionContext(httpContext, new(), new(), new());
            var actionExecutingContext = new ActionExecutingContext(actionContext, 
                new List<IFilterMetadata>(),
                new Dictionary<string, object?>(),
                controller: null);

            //Act
            checkShowStatisticsHeaderActionFilter.OnActionExecuting(actionExecutingContext);

            //Assert
            Assert.IsType<BadRequestResult>(actionExecutingContext.Result);
        }
    }
}
