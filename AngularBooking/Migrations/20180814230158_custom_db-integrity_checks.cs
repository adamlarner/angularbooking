using Microsoft.EntityFrameworkCore.Migrations;

namespace AngularBooking.Migrations
{
    public partial class custom_dbintegrity_checks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /* 
             * triggers for preventing insert/update of overlapping date ranges within 'showing' table,
             * as well as trigger for prevent duplicate bookings within showing
             */

            migrationBuilder.Sql(@"
                CREATE TRIGGER check_daterange_before_insert_showings
                BEFORE INSERT ON Showings
                WHEN EXISTS 
                    (SELECT * FROM Showings 
                     WHERE RoomId = NEW.RoomId AND 
                        ((StartTime BETWEEN NEW.StartTime AND NEW.EndTime) OR 
                         (EndTime BETWEEN NEW.StartTime AND NEW.EndTime) OR
                         (NEW.StartTime BETWEEN StartTime AND EndTime) OR
                         (NEW.EndTime BETWEEN StartTime AND EndTime)
                        )
                )
                BEGIN
                    SELECT RAISE(ABORT, 'Cannot insert Showing: Room already booked at this time');
                END;

                CREATE TRIGGER check_daterange_before_update_showings
                BEFORE UPDATE ON Showings
                WHEN EXISTS 
                    (SELECT * FROM Showings 
                     WHERE RoomId = NEW.RoomId AND Id != NEW.Id AND
                        ((StartTime BETWEEN NEW.StartTime AND NEW.EndTime) OR 
                         (EndTime BETWEEN NEW.StartTime AND NEW.EndTime) OR
                         (NEW.StartTime BETWEEN StartTime AND EndTime) OR
                         (NEW.EndTime BETWEEN StartTime AND EndTime)
                        )
                )
                BEGIN
                    SELECT RAISE(ABORT, 'Cannot update Showing: Room already booked at this time');
                END;

                CREATE TRIGGER check_unique_location_booking_in_showing_before_insert
                BEFORE INSERT ON BookingItems
                WHEN EXISTS
                    (
                     SELECT BookingItems.BookingId, BookingItems.Location, Bookings.ShowingId FROM BookingItems
                     INNER JOIN Bookings ON BookingItems.BookingId = Bookings.Id
                     WHERE BookingItems.Location = NEW.Location AND Bookings.ShowingId = (SELECT ShowingId FROM Bookings WHERE Bookings.Id = NEW.BookingId)
                    )
                BEGIN
                    SELECT RAISE(ABORT, 'Cannot insert Booking Item: The location is already taken for the showing');
                END;

                CREATE TRIGGER check_unique_location_booking_in_showing_before_update
                BEFORE UPDATE ON BookingItems
                WHEN EXISTS
                    (
                     SELECT BookingItems.BookingId, BookingItems.Location, Bookings.ShowingId FROM BookingItems
                     INNER JOIN Bookings ON BookingItems.BookingId = Bookings.Id
                     WHERE BookingItems.Id != NEW.Id AND BookingItems.Location = NEW.Location AND Bookings.ShowingId = (SELECT ShowingId FROM Bookings WHERE Bookings.Id = NEW.BookingId)
                    )
                BEGIN
                    SELECT RAISE(ABORT, 'Cannot update Booking Item: The location is already taken for the showing');
                END

            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER check_daterange_before_insert_showings; DROP TRIGGER check_daterange_before_update_showings; DROP TRIGGER check_unique_location_booking_in_showing_before_insert; DROP TRIGGER check_unique_location_booking_in_showing_before_update;");
        }
    }
}
