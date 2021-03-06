﻿using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace sodex_api_v2.Controllers
{
    [Authorize, RoutePrefix("api/reportLedger")]
    public class ReportLedgerController : ApiController
    {
        // ============
        // Data Context
        // ============
        private Data.SodexDatabaseDataContext db = new Data.SodexDatabaseDataContext();

        // ====================
        // Get Bulleted Strings
        // ====================
        public String GetCardNumberBulletedString(String cardNumber)
        {
            var firsChar = cardNumber[0];

            var slicedChars = cardNumber.Substring(1, cardNumber.Length - 1);
            var bulletedChars = new string('•', slicedChars.Length - 1);

            var lastChar = cardNumber[cardNumber.Length - 1];

            return firsChar + bulletedChars + lastChar;
        }

        // =============
        // List - Ledger
        // =============
        [HttpGet, Route("list/{cardNumber}/{dateStart}/{dateEnd}")]
        public List<Models.TrnLedger> GetLedger(string cardNumber, string dateStart, string dateEnd)
        {
            Decimal runningBalance = 0;

            List<Models.TrnLedger> newBegLedgers = new List<Models.TrnLedger>();
            var begLedgers = from d in db.TrnLedgers
                             where d.MstCard.CardNumber.Equals(cardNumber)
                             && d.LedgerDateTime.Date < Convert.ToDateTime(dateStart)
                             select new
                             {
                                 Id = 0,
                                 Document = "Beginning Balance",
                                 LedgerDateTime = Convert.ToDateTime(dateStart).ToString("MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture),
                                 CardOwner = d.MstCard.FullName,
                                 DebitAmount = d.DebitAmount,
                                 CreditAmount = d.CreditAmount,
                                 Particulars = "Beginning Balance"
                             };

            if (begLedgers.Any())
            {
                var listBegLedgers = begLedgers.ToList().OrderBy(d => d.Id);

                foreach (var begLedger in listBegLedgers)
                {
                    newBegLedgers.Add(new Models.TrnLedger()
                    {
                        Id = begLedger.Id,
                        Document = begLedger.Document,
                        LedgerDateTime = begLedger.LedgerDateTime,
                        CardOwner = begLedger.CardOwner,
                        DebitAmount = begLedger.DebitAmount,
                        CreditAmount = begLedger.CreditAmount,
                        RunningBalance = begLedger.DebitAmount - begLedger.CreditAmount,
                        Particulars = begLedger.Particulars
                    });
                }

                runningBalance += newBegLedgers.Sum(d => d.RunningBalance);
            }

            List<Models.TrnLedger> newCurrentLedgers = new List<Models.TrnLedger>();
            var currentLedgers = from d in db.TrnLedgers
                                 where d.MstCard.CardNumber.Equals(cardNumber)
                                 && d.LedgerDateTime >= Convert.ToDateTime(dateStart)
                                 && d.LedgerDateTime <= Convert.ToDateTime(dateEnd).AddHours(24)
                                 select new
                                 {
                                     Id = d.Id,
                                     Document = "Current",
                                     LedgerDateTime = d.LedgerDateTime.ToShortDateString(),
                                     CardOwner = d.MstCard.FullName,
                                     DebitAmount = d.DebitAmount,
                                     CreditAmount = d.CreditAmount,
                                     Particulars = d.Particulars
                                 };

            if (currentLedgers.Any())
            {
                var listCurrentLedgers = currentLedgers.ToList().OrderBy(d => d.Id);

                foreach (var currentLedger in listCurrentLedgers)
                {
                    runningBalance += currentLedger.DebitAmount - currentLedger.CreditAmount;

                    newCurrentLedgers.Add(new Models.TrnLedger()
                    {
                        Id = currentLedger.Id,
                        Document = currentLedger.Document,
                        LedgerDateTime = currentLedger.LedgerDateTime,
                        CardOwner = currentLedger.CardOwner,
                        DebitAmount = currentLedger.DebitAmount,
                        CreditAmount = currentLedger.CreditAmount,
                        RunningBalance = runningBalance,
                        Particulars = currentLedger.Particulars
                    });
                }
            }

            var unionLedgers = newBegLedgers.Union(newCurrentLedgers);
            if (unionLedgers.Any())
            {
                var groupedLedgers = from d in unionLedgers
                                     group d by new
                                     {
                                         d.Id,
                                         d.Document,
                                         d.LedgerDateTime,
                                         d.CardOwner,
                                         d.Particulars
                                     } into g
                                     select new Models.TrnLedger
                                     {
                                         LedgerDateTime = g.Key.LedgerDateTime,
                                         CardOwner = g.Key.CardOwner,
                                         DebitAmount = g.Sum(d => d.DebitAmount),
                                         CreditAmount = g.Sum(d => d.CreditAmount),
                                         RunningBalance = g.Sum(d => d.RunningBalance),
                                         Particulars = g.Key.Particulars
                                     };

                return groupedLedgers.ToList();
            }
            else
            {
                return new List<Models.TrnLedger>();
            }
        }

        // ====================
        // List - Debit Ledgers
        // ====================
        [HttpGet, Route("list/debit/{dateStart}/{dateEnd}")]
        public List<Models.TrnLedger> GetDebitLedger(string dateStart, string dateEnd)
        {
            var currentUser = from d in db.MstUsers where d.AspNetUserId == User.Identity.GetUserId() select d;

            var debitLedgers = from d in db.TrnLedgers
                               where d.LedgerDateTime.Date >= Convert.ToDateTime(dateStart)
                               && d.LedgerDateTime.Date <= Convert.ToDateTime(dateEnd)
                               && d.DebitAmount > 0
                               && d.CardNumber != currentUser.FirstOrDefault().MotherCardNumber
                               orderby d.Id ascending
                               select new Models.TrnLedger
                               {
                                   Id = d.Id,
                                   CardId = d.CardId,
                                   CardNumber = d.CardNumber,
                                   CardOwner = d.MstCard.FullName,
                                   LedgerDateTime = d.LedgerDateTime.ToString("MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture),
                                   DebitAmount = d.DebitAmount,
                                   Particulars = d.Particulars
                               };

            return debitLedgers.ToList();
        }

        // =====================
        // List - Credit Ledgers
        // =====================
        [HttpGet, Route("list/credit/{dateStart}/{dateEnd}")]
        public List<Models.TrnLedger> GetCreditLedger(string dateStart, string dateEnd)
        {
            var currentUser = from d in db.MstUsers where d.AspNetUserId == User.Identity.GetUserId() select d;

            var creditLedgers = from d in db.TrnLedgers
                                where d.LedgerDateTime.Date >= Convert.ToDateTime(dateStart)
                                && d.LedgerDateTime.Date <= Convert.ToDateTime(dateEnd)
                                && d.CreditAmount > 0
                                && d.CardNumber != currentUser.FirstOrDefault().MotherCardNumber
                                orderby d.Id ascending
                                select new Models.TrnLedger
                                {
                                    Id = d.Id,
                                    CardId = d.CardId,
                                    CardNumber = d.CardNumber,
                                    CardOwner = d.MstCard.FullName,
                                    LedgerDateTime = d.LedgerDateTime.ToString("MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture),
                                    CreditAmount = d.CreditAmount,
                                    Particulars = d.Particulars
                                };

            return creditLedgers.ToList();
        }

        // =====================
        // List - Credit Ledgers
        // =====================
        [HttpGet, Route("daily/summary/report/{dateAsOf}")]
        public Models.RepDailySummaryReport DailySummaryReport(string dateAsOf)
        {
            var currentUser = from d in db.MstUsers where d.AspNetUserId == User.Identity.GetUserId() select d;

            Decimal beginningBalance = 0;
            Decimal totalDebit = 0;
            Decimal totalCredit = 0;
            Decimal endingBalance = 0;
            Decimal motherCardEndingBalance = 0;

            var beginningLedgers = from d in db.TrnLedgers
                                   where d.LedgerDateTime.Date < Convert.ToDateTime(dateAsOf)
                                   && d.CardNumber != currentUser.FirstOrDefault().MotherCardNumber
                                   select d;

            if (beginningLedgers.Any())
            {
                beginningBalance = beginningLedgers.Sum(d => d.DebitAmount - d.CreditAmount);
            }

            var currentLedgers = from d in db.TrnLedgers
                                 where d.LedgerDateTime.Date == Convert.ToDateTime(dateAsOf)
                                 && d.CardNumber != currentUser.FirstOrDefault().MotherCardNumber
                                 select d;

            if (currentLedgers.Any())
            {
                totalDebit = currentLedgers.Sum(d => d.DebitAmount);
                totalCredit = currentLedgers.Sum(d => d.CreditAmount);
                endingBalance = totalDebit - totalCredit;
            }

            var motherCardLedgers = from d in db.TrnLedgers
                                    where d.LedgerDateTime.Date <= Convert.ToDateTime(dateAsOf)
                                    && d.CardNumber == currentUser.FirstOrDefault().MotherCardNumber
                                    select d;

            if (motherCardLedgers.Any())
            {
                motherCardEndingBalance = motherCardLedgers.Sum(d => d.DebitAmount - d.CreditAmount);
            }

            Models.RepDailySummaryReport dailySummaryReport = new Models.RepDailySummaryReport
            {
                BeginningBalance = beginningBalance,
                TotalDebit = totalDebit,
                TotalCredit = totalCredit,
                EndingBalance = beginningBalance + (totalDebit - totalCredit),
                MotherCardEndingBalance = motherCardEndingBalance
            };

            return dailySummaryReport;
        }
    }
}
