using SmsIntegration.BLL.Enums;
using SmsIntegration.BLL.Interfaces;
using SmsIntegration.DAL.IRepository;
using SmsIntegration.DAL.Repository;
using SmsIntegration.Database.Models;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.Serialization;

namespace SmsIntegration.BLL.Services
{
    public class SmsServices : ISmsServices
    {
        private readonly ISmsProviderRepository smsProviderRepository;
        private readonly ISmsRepository smsRepository;
        private readonly ISmsFlowRepository smsFlowRepository;
        public SmsServices(
            ISmsProviderRepository smsProviderRepository,
            ISmsRepository smsRepository,
            ISmsFlowRepository smsFlowRepository)
        {
            this.smsProviderRepository = smsProviderRepository;
            this.smsRepository = smsRepository;
            this.smsFlowRepository = smsFlowRepository;
        }
        public async Task<long> CreateMessage(string phoneNumber, string messageText)
        {
            try
            {
                var messageToSend = new Smss()
                {
                    PhoneNumber = phoneNumber,
                    MessageText = messageText,
                    Status = (int)SmsFlowStatus.Created,
                    AttamptCount = 0,
                    CreateDate = DateTime.Now,
                    SmsFlows = new List<SmsFlow>()
                    {
                        new SmsFlow()
                        {
                            Status = (int)SmsFlowStatus.Created,
                            Data = "Message Created",
                            CreateDate = DateTime.Now,
                        }
                    }
                };
                await smsRepository.AddAsync(messageToSend);
                return messageToSend.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task SendMessage(long messageId)
        {
            var selectedSmsProvider = await smsProviderRepository.SelecteSmsProvider();

            if (selectedSmsProvider == null)
            {
                return;
            }
            var messageToSend = await smsRepository.GetByIdAsync(messageId);

            messageToSend.Status = (int)SmsFlowStatus.Started;
            messageToSend.AttamptCount++;
            messageToSend.SmsFlows.Add(new SmsFlow
            {
                ProviderId = selectedSmsProvider.Id,
                Status = (int)SmsFlowStatus.Started,
                Data = "Start sending message",
                CreateDate = DateTime.Now
            });
            await smsRepository.UpdateAsync(messageToSend);

            try
            {
                Type type = typeof(ISmsProvider);

                var providerClass = Assembly
                    .GetExecutingAssembly()
                    .GetTypes()
                    .Where(p =>
                            type.IsAssignableFrom(p) &&
                            p.Name == selectedSmsProvider.ImplementatorName)
                    .FirstOrDefault();

                if (providerClass == null)
                {
                    throw new Exception("Implementator Not Found");
                }

                ISmsProvider smsProvider = (ISmsProvider)Activator
                    .CreateInstance(providerClass, null);

                await smsProvider.InitializeMessageProvider(selectedSmsProvider);
                (bool sendStatus, string message) = await smsProvider.SendMessage(messageToSend.PhoneNumber, messageToSend.MessageText);

                if (!sendStatus)
                {
                    throw new Exception(message);
                }

                await smsFlowRepository.AddAsync(new SmsFlow()
                {
                    SmsId = messageToSend.Id,
                    Status = (int)SmsFlowStatus.Sended,
                    Data = "Sended",
                    CreateDate = DateTime.Now,
                });

            }
            catch (Exception ex)
            {
                messageToSend.Status = (int)SmsFlowStatus.Error;

                await smsRepository.UpdateAsync(messageToSend);

                await smsFlowRepository.AddAsync(new SmsFlow()
                {
                    SmsId = messageToSend.Id,
                    Status = (int)SmsFlowStatus.Error,
                    ProviderId = selectedSmsProvider?.Id,
                    Data = ex.Message,
                    CreateDate = DateTime.Now,
                });

            }
        }
    }
}
