using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Trepub.Common.Utils
{
    public class CalendarUtils
    {
        static public bool IsInDifferentMonths(DateTime st1, DateTime st2)
        {
            PersianCalendar pc = new PersianCalendar();
            return pc.GetMonth(st1) != pc.GetMonth(st2);
        }
        static public int GetDaysInMonth(DateTime st1)
        {
            PersianCalendar pc = new PersianCalendar();
            return pc.GetDaysInMonth(pc.GetYear(st1), pc.GetMonth(st1));
        }
        static public DateTime GetLastDateOfMonthInShamsi(int year, int month)
        {
            PersianCalendar pc = new PersianCalendar();
            var firstdate = CalendarUtils.GetMiladiByShamsi(year, month, 1);
            var daysinmonth = CalendarUtils.GetDaysInMonth(firstdate);
            return CalendarUtils.GetMiladiByShamsi(year, month, daysinmonth);
        }
        static public (int year, int month, int day) GetShamsiByMiladi(DateTime st1)
        {
            PersianCalendar pc = new PersianCalendar();
            return (pc.GetYear(st1), pc.GetMonth(st1), pc.GetDayOfMonth(st1));
        }
        static public string GetShamsiByMiladiStr(DateTime st1)
        {
            var ret1 = GetShamsiByMiladi(st1);
            return $"{ret1.year}/{ret1.month}/{ret1.day}";
        }
        static public string GetShamsiByMiladiStrWithTime(DateTime st1)
        {
            var ret1 = GetShamsiByMiladi(st1);
            var retstr = $"{ret1.year}/{ret1.month:D2}/{ret1.day:D2} {st1.Hour:D2}:{st1.Minute:D2}:{st1.Second:D2}";
            return retstr;
        }
        static public DateTime PCAddMonths(DateTime st1, int n)
        {
            PersianCalendar pc = new PersianCalendar();
            return pc.AddMonths(st1, n);
        }
        static public DateTime GetStartOfNextNMonth(DateTime st1, int n)
        {
            PersianCalendar pc = new PersianCalendar();
            var nextm1 = pc.AddMonths(st1, n);
            return new DateTime(pc.GetYear(nextm1), pc.GetMonth(nextm1), 1, pc);//.AddSeconds(-1);
        }
        static public DateTime GetStartOfNextMonth(DateTime st1)
        {
            PersianCalendar pc = new PersianCalendar();
            var nextm1 = pc.AddMonths(st1, 1);
            return new DateTime(pc.GetYear(nextm1), pc.GetMonth(nextm1), 1, pc);//.AddSeconds(-1);
        }
        static public DateTime GetStartOfThisMonth(DateTime st1)
        {
            PersianCalendar pc = new PersianCalendar();
            return new DateTime(pc.GetYear(st1), pc.GetMonth(st1), 1, pc);
        }
        static public DateTime GetMiladiByShamsi(int year, int month, int day)
        {
            PersianCalendar pc = new PersianCalendar();
            return new DateTime(year, month, day, pc);
        }
    }
}
