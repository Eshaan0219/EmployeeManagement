using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.Data;
using EmployeeManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore.Query.Internal;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Cryptography;
using Azure.Core;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;
using System.Net.Mail;

namespace EmployeeManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        static bool Check(string strToCheck)
        {
            Regex rg = new Regex(@"^[a-zA-Z0-9\s.]*$");
            char[] delimiters = new char[] { ' ' };
            if (strToCheck.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Length > 1)
            {
                return false;
            }
            return rg.IsMatch(strToCheck);
        }
        static bool CheckCity(string strToCheck)
        {
            Regex rg = new Regex(@"^[a-zA-Z0-9\s.]*$");
            
            return rg.IsMatch(strToCheck);
        }
        static bool IsValid(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
        static bool CheckPin(string strToCheck)
        {
            Regex rg = new Regex(@"^[0-9]{6}$");

            return rg.IsMatch(strToCheck);

        }
        
        static bool Checkroll(string strToCheck)
        {
            Regex rg = new Regex(@"^[a-zA-Z0-9]*$");

            return rg.IsMatch(strToCheck);
        }
        static bool IsPhoneNumber(string strToCheck)
        {
            Regex rg = new Regex(@"^[0-9]{10}$");

            return rg.IsMatch(strToCheck);
        }
        static bool CheckD(DateTime d)
        {
            if (d >= DateTime.Today)
            {
                return false;
            }
            return true;
        }

        private readonly EmployeeManagementDbContext dbContext;
        public EmployeeController(EmployeeManagementDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var res = await dbContext
            .employees
            .FromSqlInterpolated($"GetAllEmployeesSP").ToListAsync();
            return Ok(res);
        }
        
        [HttpGet]
        [Route("Filter")]
        public async Task<IActionResult> GetEmployee(int id,string? Fname,string? Lname,string? Email,string? PhNo,string? City,string? Pincode,decimal sal)
        {
            /*var get = await dbContext
            .employees
            .FromSqlInterpolated($"GetEmployeesSP {id}").ToListAsync();
            if (get.IsNullOrEmpty())
            {
                return BadRequest("Record Not Found");
            }*/
            var res = await dbContext
            .employees
            .FromSqlInterpolated($"GetEmployeesSP {id},{Fname},{Lname},{Email},{PhNo},{City},{Pincode},{sal}").ToListAsync();
            return Ok(res);
        }
        
        [HttpPost]
        public async Task<IActionResult> AddEmployee(AddEmployeeDTO requests)
        {
            var Da = await dbContext
            .dates
            .FromSqlInterpolated($"checkDate {requests.DOB}").ToListAsync();
            //return Ok(Da[0]);
            if (Da[0].D < 1918)
            {
                return BadRequest("Not Eligible Age");
            }


            if (!Check(requests.FName))
            {
                return BadRequest("Fname Should be a single word and shouldnt contain any special characters");
            }
            if (!Check(requests.Lname))
            {
                return BadRequest("Fname Should be a single word and shouldnt contain any special characters");
            }
            if (!CheckD(requests.HireDate))
            {
                return BadRequest("Invalid Hiredate entered");
            }
            if (!IsValid(requests.Email))
            {
                return BadRequest("Invalid Email entered");
            }
            if (!IsPhoneNumber(requests.PhNo))
            {
                return BadRequest("Invalid Phone Number");
            }
            if (!CheckPin(requests.PinCode))
            {
                return BadRequest("Invalid Pin Code");
            }
            if (!CheckCity(requests.City))
            {
                return BadRequest("Invalid City Name");
            }
            if (requests.Salary <= 0)
            {
                return BadRequest("Invalid Salary");
            }
            if (requests.EmpId <= 0)
            {
                return BadRequest("Invalid Id");
            }
            var getphone = await dbContext
            .employees
            .FromSqlInterpolated($"GetEmployeesPhoneSPP {requests.PhNo}").ToListAsync();
            if (!getphone.IsNullOrEmpty())
            {
                return BadRequest("Dupelicate Phno in Table");
            }
            var getmail = await dbContext
            .employees
            .FromSqlInterpolated($"GetEmployeesEmailSPP {requests.Email}").ToListAsync();
            if (!getmail.IsNullOrEmpty())
            {
                return BadRequest("Dupelicate Email in Table");
            }
            var get = await dbContext
            .employees
            .FromSqlInterpolated($"GetEmployeesSPP {requests.EmpId}").ToListAsync();
            if (!get.IsNullOrEmpty())
            {
                return BadRequest("Dupelicate Id in Table");
            }
            var res = await dbContext
           .employees
           .FromSqlInterpolated($"AddEmployeeSP  {requests.EmpId},{requests.FName},{requests.Lname},{requests.Email},{requests.PhNo},{requests.City},{requests.PinCode},{requests.DOB},{requests.HireDate},{requests.Salary}").ToListAsync();

            
            return Ok();
        }
        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateEmployee([FromRoute] int id, UpdateEmployeeDTO requests)
        {
            
            var get = await dbContext
            .employees
            .FromSqlInterpolated($"GetEmployeesSPP {id}").ToListAsync();
            if (get.IsNullOrEmpty())
            {
                return BadRequest("Record Not Found");
            }
            if (!Check(requests.FName))
            {
                return BadRequest("Fname Should be a single word and shouldnt contain any special characters");
            }
            if (!Check(requests.Lname))
            {
                return BadRequest("Fname Should be a single word and shouldnt contain any special characters");
            }
            if (!CheckD(requests.HireDate))
            {
                return BadRequest("Invalid date entered");
            }
            if (!IsValid(requests.Email))
            {
                return BadRequest("Invalid Email entered");
            }
            if (!IsPhoneNumber(requests.PhNo))
            {
                return BadRequest("Invalid Phone Number");
            }
            if (requests.Salary <= 0)
            {
                return BadRequest("Invalid Salary");
            }


            var getphone = await dbContext
            .employees
            .FromSqlInterpolated($"GetEmployeesPhoneSPP {requests.PhNo}").ToListAsync();
            if (!getphone.IsNullOrEmpty())
            {
                return BadRequest("Dupelicate Phno in Table");
            }
            var getmail = await dbContext
            .employees
            .FromSqlInterpolated($"GetEmployeesEmailSPP {requests.Email}").ToListAsync();
            if (!getmail.IsNullOrEmpty())
            {
                return BadRequest("Dupelicate Email in Table");
            }
            

            var res = await dbContext
            .employees
            .FromSqlInterpolated($"UpdateEmployeeSP  {id},{requests.FName},{requests.Lname},{requests.Email},{requests.PhNo},{requests.City},{requests.PinCode},{requests.DOB},{requests.HireDate},{requests.Salary}").ToListAsync();
            return Ok();
        }
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteEmployee([FromRoute] int id)
        {
            var get = await dbContext
            .employees
            .FromSqlInterpolated($"GetEmployeesSPP {id}").ToListAsync();
            if (get.IsNullOrEmpty())
            {
                return BadRequest("Record Not Found");
            }
            var res = await dbContext
            .employees
            .FromSqlInterpolated($"DeleteEmployeeSP  {id}").ToListAsync();
            return Ok();
        }
    }
}
