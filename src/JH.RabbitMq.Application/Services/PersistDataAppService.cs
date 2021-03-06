﻿using FluentValidation.Results;
using JH.RabbitMq.Application.Services.Interfaces;
using JH.RabbitMq.Application.ViewModels;
using JH.RabbitMq.Domain;
using JH.RabbitMq.Domain.Services.Interfaces;
using JH.RabbitMq.Domain.UoW;
using System;
using System.Linq;

namespace JH.RabbitMq.Application.Services
{

    public class PersistDataAppService
    : AppServiceBase, IPersistDataAppService
    {
        private readonly ITemperatureDataStorageService _historicService;
        public PersistDataAppService(IUnitOfWork uow,
            ITemperatureDataStorageService historicService) : base(uow)
        {
            _historicService = historicService;
        }

        public void PersistData(PersistDataViewModel viewModel, Action<string> notifyError)
        {
            var locale = viewModel.Data;

            foreach (var item in locale.data)
            {
                var data = TemperatureDataStorage.Factory.CreateNew(locale.name,
                     locale.state,
                     locale.country,
                     item.date,
                     item.rain.precipitation,
                     item.temperature.temperature);

                if (data.IsValid())
                    _historicService.Add(data);
                else
                    data.Notifications.ToList().ForEach((ValidationFailure error) => notifyError(error.ErrorMessage));

            }


            Commit();

        }
    }
}
