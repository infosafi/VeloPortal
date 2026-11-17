using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeloPortal.Application.Interfaces.FacilityManagement;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Entities.FacilityManagement;
using VeloPortal.Domain.Extensions;
using VeloPortal.Infrastructure.Data.DataContext;
using VeloPortal.Infrastructure.Data.SPHelper;

namespace VeloPortal.Infrastructure.Data.Repositories.FacilityManagement
{
    public class ReservationInfRepository : IReservationInf
    {
        private readonly IDbContextFactory<VeloPortalDbContext> _dbContextFactory;
        private readonly ILogger<ReservationInfRepository> _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly SPProcessAccess? _spProcessAccess;
        public ReservationInfRepository(IDbContextFactory<VeloPortalDbContext> dbContextFactory, ILogger<ReservationInfRepository> logger, IMapper mapper, IConfiguration configuration)
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
            _mapper = mapper;
            _configuration = configuration;

            var connectionString = _configuration.GetConnectionString(DefaultSettings.DefaultDbconnection);
            _spProcessAccess = new SPProcessAccess(connectionString);
        }
        public async Task<ReservationInf?> InsertOrUpdateReservationInf(ReservationInf obj)
        {
            if (obj == null)
                return null;

            try
            {
                using var dbContext = _dbContextFactory.CreateDbContext();
                await using var transaction = await dbContext.Database.BeginTransactionAsync();

                var fromDate = obj.fromdate != default ? obj.fromdate : DateTime.UtcNow;
                var toDate = obj.todate != default ? obj.todate : DateTime.UtcNow;

                if (fromDate >= toDate)
                {
                    throw new Exception("From date must be earlier than To date.");
                }

                var existingData = await dbContext.ReservationInf.FirstOrDefaultAsync(a => a.room_booking_id == obj.room_booking_id);
                bool datesChanged = existingData == null || existingData.fromdate != fromDate || existingData.todate != toDate || existingData.acccode != obj.acccode
                                    || existingData.rescode != obj.rescode || (existingData.is_cancel && !obj.is_cancel);

                if (datesChanged && await dbContext.ReservationInf.AnyAsync(r => r.room_booking_id != obj.room_booking_id && r.comcod == obj.comcod && r.acccode == obj.acccode
                    && r.rescode == obj.rescode && r.is_cancel == false && r.fromdate <= toDate && fromDate <= r.todate))
                {
                    throw new Exception($"A reservation already exists for this time slot ({fromDate} – {toDate}). Please choose a different time range.");
                }

                if (existingData != null)
                {
                    existingData.fromdate = obj.fromdate;
                    existingData.todate = obj.todate;
                    existingData.purpose = obj.purpose;
                    existingData.remarks = obj.remarks;
                    existingData.presense = obj.presense;
                    existingData.is_cancel = obj.is_cancel;
                    existingData.is_approved = obj.is_approved;
                    existingData.acccode = obj.acccode;
                    existingData.rescode = obj.rescode;
                    existingData.booking_fee = obj.booking_fee;
                    existingData.review_by = obj.review_by;
                    existingData.review_date = obj.review_date;
                    existingData.review_note = obj.review_note;

                    await dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return existingData;
                }

                var newData = new ReservationInf
                {
                    comcod = obj.comcod,
                    acccode = obj.acccode,
                    rescode = obj.rescode,
                    requester_role = obj.requester_role,
                    reqdate = obj.reqdate != default ? obj.reqdate : DateTime.UtcNow,
                    fromdate = obj.fromdate != default ? obj.fromdate : DateTime.UtcNow,
                    todate = obj.todate != default ? obj.todate : DateTime.UtcNow,
                    purpose = obj.purpose,
                    remarks = obj.remarks,
                    presense = obj.presense,
                    reqsource = obj.reqsource,
                    is_approved = obj.is_approved,
                    is_cancel = obj.is_cancel,
                    created_date = obj.created_date != default ? obj.created_date : DateTime.UtcNow,
                    created_by = obj.created_by,
                    review_date = obj.review_date != default ? obj.review_date : DateTime.UtcNow,
                    review_by = obj.review_by,
                    review_note = obj.review_note,
                    booking_host = obj.booking_host,
                    booking_fee = obj.booking_fee
                };

                await dbContext.ReservationInf.AddAsync(newData);
                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return newData;
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex.InnerException ?? ex);
                _logger.LogError(ex, "Reservation Information update failed");
                return null;
            }
        }


        public async Task<List<DtoReqAmenitiesSave>?> InsertOrUpdateReservationAmenities(List<DtoReqAmenitiesSave> list)
        {
            if (list == null || list.Count == 0)
                return null;
            if (list.Any(x => x.room_booking_id <= 0 || string.IsNullOrEmpty(x.comcod) || string.IsNullOrEmpty(x.gencode)))
            {
                _logger.LogWarning("Invalid input data. BookingAmenitiesId, RoomBookingId, Comcod, and Gencode must be provided.");
                return null;
            }
            try
            {
                using var dbContext = _dbContextFactory.CreateDbContext();
                await using var transaction = await dbContext.Database.BeginTransactionAsync();

                var comcods = list.Select(x => x.comcod).Distinct().ToList();
                var bookingIds = list.Select(x => x.room_booking_id).Distinct().ToList();
                var gencodes = list.Select(x => x.gencode).Distinct().ToList();
                var existing = await dbContext.ReqAmenities.Where(x => comcods.Contains(x.comcod) && bookingIds.Contains(x.room_booking_id) && gencodes.Contains(x.gencode)).ToListAsync();

                var result = new List<ReqAmenities>();
                foreach (var obj in list)
                {
                    var cur = existing.FirstOrDefault(x => x.room_booking_id == obj.room_booking_id
                        && x.comcod == obj.comcod
                        && x.gencode == obj.gencode);

                    if (obj.is_cancel)
                    {
                        if (cur != null && cur.booking_amenities_id > 0)
                        {
                            dbContext.ReqAmenities.Remove(cur);
                            existing.Remove(cur);
                        }
                        continue;
                    }

                    if (cur != null)
                    {
                        cur.frequncy = obj.frequncy;
                        cur.charge_amt = obj.charge_amt;
                        cur.created_date = obj.created_date != default ? obj.created_date : DateTime.UtcNow;
                        cur.created_by = obj.created_by;
                        result.Add(cur);
                    }
                    else
                    {
                        var n = new ReqAmenities
                        {
                            room_booking_id = obj.room_booking_id,
                            comcod = obj.comcod,
                            gencode = obj.gencode,
                            frequncy = obj.frequncy,
                            charge_amt = obj.charge_amt,
                            created_date = obj.created_date != default ? obj.created_date : DateTime.UtcNow,
                            created_by = obj.created_by
                        };
                        await dbContext.ReqAmenities.AddAsync(n);
                        existing.Add(n);
                        result.Add(n);
                    }
                }

                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return result.Select(x => new DtoReqAmenitiesSave
                {
                    booking_amenities_id = x.booking_amenities_id,
                    room_booking_id = x.room_booking_id,
                    comcod = x.comcod,
                    gencode = x.gencode,
                    frequncy = x.frequncy,
                    charge_amt = x.charge_amt,
                    created_date = x.created_date,
                    created_by = x.created_by
                }).ToList();
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex.InnerException ?? ex);
                _logger.LogError(ex, "Reservation Amenities update failed");
                return null;
            }
        }

        public async Task<DtoReservationInf?> GetReservationInfList(string? comcod, string? fromDate, string? toDate, string? acccode)
        {
            try
            {
                if (_spProcessAccess == null)
                {
                    _logger.LogWarning("_spProcessAccess is not initialized.");
                    return null;
                }

                DataSet? ds = _spProcessAccess.GetTransInfo20(comcod ?? "", "itv_fms.SP_FACILITY_MGT", "Get_Reservation_Info_List", fromDate ?? "", toDate ?? "", acccode ?? "");
                if (ds == null || ds.Tables.Count == 0)
                    return null;

                return new DtoReservationInf
                {
                    Reservation = ds.Tables[0].DataTableToDynamicList(),
                    Amenities = ds.Tables.Count > 1 ? ds.Tables[1].DataTableToDynamicList() : new List<dynamic>()
                };
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                _logger.LogError(ex, "List Retrieval Failed");
                return null;
            }
        }
    }
}
