using BrazilianHedgeFunds.API.Repository.Interfaces;
using BrazilianHedgeFunds.API.Services;
using BrazilianHedgeFunds.API.Services.Dtos;
using BrazilianHedgeFunds.API.Services.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrazilianHedgeFund.API.Tests
{
    public class HedgeFundServiceTest
    {
        private DateTime START_DATE;
        private DateTime END_DATE;
        private DateTime DEFAULT_DATE;
        private DateTime FAIL_START_DATE;
        private DateTime FAIL_END_DATE;

        private const string DEFAULT_CNPJ = "00.280.302/0001-60";
        private const string EX_MESSAGE = "Error";

        private Mock<IConfiguration> _configuration;
        private Mock<ILogger<HedgeFundService>> _logger;
        private Mock<IHedgeFundRecordRepository> _hedgeFundRepository;

        [SetUp]
        public void Setup()
        {
            START_DATE = new DateTime(2017, 01, 01);
            END_DATE = DateTime.Now;
            DEFAULT_DATE = new DateTime();

            FAIL_START_DATE = new DateTime(2000, 01, 01);
            FAIL_END_DATE = DateTime.Now.AddMonths(1);

            _configuration = new Mock<IConfiguration>();
            _logger = new Mock<ILogger<HedgeFundService>>();
            _hedgeFundRepository = new Mock<IHedgeFundRecordRepository>();

            _configuration.Setup(c => c["Params:DefaulPageSize"]).Returns("100");
            _configuration.Setup(c => c["Params:OldesYearToProcess"]).Returns("2017");
        }

        [TestCase(0, 0, true)]
        [TestCase(0, 1, true)]
        [TestCase(1, 0, true)]
        [TestCase(1, 1, true)]
        [TestCase(2, 0, true)]
        [TestCase(2, 1, true)]
        [TestCase(0, 0, false)]
        [TestCase(0, 1, false)]
        [TestCase(1, 0, false)]
        [TestCase(1, 1, false)]
        [TestCase(2, 0, false)]
        [TestCase(2, 1, false)]
        public void GetFundsByIdSuccessfully(int pageSize, int pageIndex, bool defaultDates)
        {
            int listSize = 2;
            HedgeFundInDto inDto;

            if (defaultDates)
                inDto = GetHedgeFundInDto(DEFAULT_DATE, DEFAULT_DATE, DEFAULT_CNPJ, pageSize, pageIndex);
            else
                inDto = GetHedgeFundInDto(START_DATE, END_DATE, DEFAULT_CNPJ, pageSize, pageIndex);

            _hedgeFundRepository.Setup(repo => repo.GetFundsBy(It.IsAny<HedgeFundInDto>())).Returns(Task.FromResult(GetListFundsRecord(listSize)));

            var service = new HedgeFundService(_logger.Object, _configuration.Object, _hedgeFundRepository.Object);

            var outDto = service.GetFundsBy(inDto).Result;

            Assert.IsFalse(outDto.IsInvalid);
            Assert.IsTrue(string.IsNullOrWhiteSpace(outDto.ErrorMessage));
            Assert.NotZero(outDto.HedgeFundRecords.Count);
            Assert.AreEqual(outDto.PageInfo.TotalItemCount, listSize);
        }

        [Test]
        public void GetFundsSuccessfullyOrderedByDate()
        {
            var inDto = GetHedgeFundInDto(START_DATE, END_DATE);

            var fundRecords = GetListFundsRecord(10).OrderByDescending(f => f.RecordDate).ToList();
            _hedgeFundRepository.Setup(repo => repo.GetFundsBy(It.IsAny<HedgeFundInDto>())).Returns(Task.FromResult(fundRecords));

            var service = new HedgeFundService(_logger.Object, _configuration.Object, _hedgeFundRepository.Object);

            var outDto = service.GetFundsBy(inDto).Result;
            var records = outDto.HedgeFundRecords;

            Assert.IsFalse(outDto.IsInvalid);
            Assert.IsTrue(string.IsNullOrWhiteSpace(outDto.ErrorMessage));

            for (int i = 0; i < records.Count - 1; i++)
                Assert.GreaterOrEqual(records[i+1].RecordDate, records[i].RecordDate);

        }

        [TestCase(true, false, 10)]
        [TestCase(true, true, 10)]
        [TestCase(true, true, 200)]
        [TestCase(false, false, 10)]
        [TestCase(false, true, 10)]
        [TestCase(false, true, 200)]
        public void TryGetFundsButGetInvalidOutputBecauseDataValidationFail(bool failStartDate, bool failEndDate, int pageSize)
        {
            var startDate = failStartDate ? FAIL_START_DATE : START_DATE;
            var endDate = failEndDate ? FAIL_END_DATE : END_DATE;

            var inDto = GetHedgeFundInDto(startDate, endDate, DEFAULT_CNPJ, pageSize, pageSize);

            var service = new HedgeFundService(_logger.Object, _configuration.Object, _hedgeFundRepository.Object);

            var outDto = service.GetFundsBy(inDto).Result;

            Assert.IsTrue(outDto.IsInvalid);
            Assert.IsFalse(string.IsNullOrWhiteSpace(outDto.ErrorMessage));
            Assert.Null(outDto.HedgeFundRecords);
            Assert.Null(outDto.PageInfo);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void TryGetFundsWithStartDateInvalidAndGetInvalidOutput(bool startDateBigger)
        {
            var startDate = startDateBigger ? DateTime.Now.AddMonths(10) : FAIL_START_DATE;

            var inDto = GetHedgeFundInDto(startDate, DateTime.Now);

            var service = new HedgeFundService(_logger.Object, _configuration.Object, _hedgeFundRepository.Object);

            var outDto = service.GetFundsBy(inDto).Result;

            Assert.IsTrue(outDto.IsInvalid);
            Assert.IsFalse(string.IsNullOrWhiteSpace(outDto.ErrorMessage));
            Assert.Null(outDto.HedgeFundRecords);
            Assert.Null(outDto.PageInfo);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void TryGetFundsWithEndDateInvalidAndGetInvalidOutput(bool endDateBigger)
        {
            var endDate = endDateBigger ? FAIL_END_DATE : FAIL_START_DATE;

            var inDto = GetHedgeFundInDto(DateTime.Now, endDate);

            var service = new HedgeFundService(_logger.Object, _configuration.Object, _hedgeFundRepository.Object);

            var outDto = service.GetFundsBy(inDto).Result;

            Assert.IsTrue(outDto.IsInvalid);
            Assert.IsFalse(string.IsNullOrWhiteSpace(outDto.ErrorMessage));
            Assert.Null(outDto.HedgeFundRecords);
            Assert.Null(outDto.PageInfo);
        }

        [Test]
        public void TryGetFundsRecordsWithInvalidPageSizeAndGetInvalidOutput()
        {
            var inDto = GetHedgeFundInDto(START_DATE, END_DATE, pageSize: 200);

            var service = new HedgeFundService(_logger.Object, _configuration.Object, _hedgeFundRepository.Object);

            var outDto = service.GetFundsBy(inDto).Result;

            Assert.IsTrue(outDto.IsInvalid);
            Assert.IsFalse(string.IsNullOrWhiteSpace(outDto.ErrorMessage));
            Assert.Null(outDto.HedgeFundRecords);
            Assert.Null(outDto.PageInfo);
        }

        [Test]
        public void TryGetFundRecordsWithInputFilterAndItDoesntExist()
        {
            var inDto = GetHedgeFundInDto(START_DATE, END_DATE);

            _hedgeFundRepository.Setup(r => r.GetFundsBy(It.IsAny<HedgeFundInDto>())).Returns(Task.FromResult(GetListFundsRecord(0)));

            var service = new HedgeFundService(_logger.Object, _configuration.Object, _hedgeFundRepository.Object);

            var outDto = service.GetFundsBy(inDto).Result;

            Assert.IsFalse(outDto.IsInvalid);
            Assert.IsTrue(string.IsNullOrWhiteSpace(outDto.ErrorMessage));
            Assert.IsEmpty(outDto.HedgeFundRecords);
            Assert.Null(outDto.PageInfo);
        }

        [Test]
        public void TryGetFundRecordsButRepositoryThrowsException()
        {
            var inDto = GetHedgeFundInDto(START_DATE, END_DATE);

            _hedgeFundRepository.Setup(r => r.GetFundsBy(It.IsAny<HedgeFundInDto>())).Throws(new Exception(EX_MESSAGE));

            var service = new HedgeFundService(_logger.Object, _configuration.Object, _hedgeFundRepository.Object);

            var outDto = service.GetFundsBy(inDto).Result;

            Assert.IsTrue(outDto.IsInvalid);
            Assert.AreEqual(outDto.ErrorMessage, EX_MESSAGE);
            Assert.Null(outDto.HedgeFundRecords);
            Assert.Null(outDto.PageInfo);
        }

        [Test]
        public void TryGetFundRecordsWithPageIndexOutOfLimitOfContextAndGetEmptyList()
        {
            int totalRecords = 7;
            var inDto = GetHedgeFundInDto(START_DATE, END_DATE, pageSize: 10, pageIndex: 10);

            _hedgeFundRepository.Setup(r => r.GetFundsBy(It.IsAny<HedgeFundInDto>())).Returns(Task.FromResult(GetListFundsRecord(totalRecords)));

            var service = new HedgeFundService(_logger.Object, _configuration.Object, _hedgeFundRepository.Object);

            var outDto = service.GetFundsBy(inDto).Result;

            Assert.IsFalse(outDto.IsInvalid);
            Assert.IsTrue(string.IsNullOrWhiteSpace(outDto.ErrorMessage));
            Assert.IsEmpty(outDto.HedgeFundRecords);
            Assert.AreEqual(outDto.PageInfo.TotalItemCount, totalRecords);
        }

        private List<HedgeFundRecord> GetListFundsRecord(int size)
        {
            var funds = new List<HedgeFundRecord>();
            var fund = new HedgeFundRecord
            {
                CNPJ = DEFAULT_CNPJ,
                DayInvestmentsTotalValue = 10,
                DayWithdrawalsTotalValue = 10,
                Id = DEFAULT_CNPJ,
                InvestorsTotal = 1000,
                PortfolioTotalValue = 1000,
                QuotaValue = 1000,
                WorthValue = 1000,
                RecordDate = START_DATE
            };

            for (int i = 1; i <= size; i++)
            {
                fund.RecordDate.AddMonths(i);
                funds.Add(fund);
            }

            return funds;
        }

        private HedgeFundInDto GetHedgeFundInDto(DateTime start, DateTime end, string cnpj = DEFAULT_CNPJ, int pageSize = 0, int pageIndex = 0)
        {
            return new HedgeFundInDto
            {
                CNPJ = cnpj,
                StartDate = start,
                EndDate = end,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
    }
}
