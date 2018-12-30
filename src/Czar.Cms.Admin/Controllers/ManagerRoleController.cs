﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Czar.Cms.Models;
using Czar.Cms.IRepository;
using Czar.Cms.Admin.ResultModel;
using System.Text;
using Czar.Cms.Admin.Validation;
using FluentValidation.Results;
using Czar.Cms.ViewModels;
using Czar.Cms.Core.Extensions;
using Czar.Cms.Core.Helper;
using AutoMapper;

namespace Czar.Cms.Admin.Controllers
{
    public class ManagerRoleController : BaseController
    {
        private readonly IManagerRoleRepository _managerRoleRepository;
        private readonly IMapper _mapper;

        public ManagerRoleController(IManagerRoleRepository managerRoleRepository, IMapper mapper)
        {
            _managerRoleRepository = managerRoleRepository;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        
        public string LoadData([FromQuery]ManagerRoleRequestModel model)
        {
            string conditions = "where 1=1 ";
            if (!model.Key.IsNullOrWhiteSpace())
            {
                conditions += $"and RoleName like '%{model.Key}%'";
            }
            var result = new TableDataModel
            {
                count = _managerRoleRepository.RecordCount(conditions),
                data = _managerRoleRepository.GetListPaged(model.Page, model.Limit, conditions, "Id desc"),
            };
            return JsonHelper.Serialize(result);
        }

        [HttpGet]
        public IActionResult AddOrModify()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrModify([FromForm]ManagerRoleAddOrModifyModel item)
        {
            var result = new BaseResult();
            ManagerRoleValidation validationRules = new ManagerRoleValidation();
            ValidationResult results = validationRules.Validate(item);
            if (results.IsValid)
            {
                ManagerRole managerRole=_mapper.Map<ManagerRole>(item);
                if (managerRole.Id == 0)
                {
                    //TODO ADD
                    managerRole = new ManagerRole();
                    managerRole.AddManagerId = 1;
                    managerRole.IsDelete = false;
                    managerRole.AddTime = DateTime.Now;
                    if (_managerRoleRepository.Insert(managerRole) > 0)
                    {
                        result.ResultCode = ResultCodeAddMsgKeys.CommonObjectSuccessCode;
                        result.ResultMsg = ResultCodeAddMsgKeys.CommonObjectSuccessMsg;

                    }
                    else
                    {
                        result.ResultCode = ResultCodeAddMsgKeys.CommonExceptionCode;
                        result.ResultMsg = ResultCodeAddMsgKeys.CommonExceptionMsg;
                    }
                }
                else
                {
                    //TODO Modify
                    managerRole = _managerRoleRepository.Get(item.Id);
                    if (managerRole != null)
                    {

                        managerRole.ModifyManagerId = 1;
                        managerRole.ModifyTime = DateTime.Now;
                        if (_managerRoleRepository.Update(managerRole) > 0)
                        {
                            result.ResultCode = ResultCodeAddMsgKeys.CommonObjectSuccessCode;
                            result.ResultMsg = ResultCodeAddMsgKeys.CommonObjectSuccessMsg;
                        }
                        else
                        {
                            result.ResultCode = ResultCodeAddMsgKeys.CommonExceptionCode;
                            result.ResultMsg = ResultCodeAddMsgKeys.CommonExceptionMsg;
                        }
                    }
                    else
                    {
                        result.ResultCode = ResultCodeAddMsgKeys.CommonFailNoDataCode;
                        result.ResultMsg = ResultCodeAddMsgKeys.CommonFailNoDataMsg;
                    }
                   
                    

                }
            }
            else
            {
                result.ResultCode = ResultCodeAddMsgKeys.CommonModelStateInvalidCode;
                result.ResultMsg = results.ToString("||");
            }
            return Json(result);
        }
    }
}