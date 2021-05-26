using DeskBooker.Core.Domain;
using DeskBooker.Core.Processor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DeskBooker.Web.Pages
{
    public class BookDeskModel : PageModel
    {
        private IDeskBookingRequestProcessor _bookingRequestProcessor;

        public BookDeskModel(IDeskBookingRequestProcessor bookingRequestProcessor)
        {
            _bookingRequestProcessor = bookingRequestProcessor;
        }

        [BindProperty]
        public DeskBookingRequest DeskBookingRequest { get; set; }

        public void OnPost()
        {
            _bookingRequestProcessor.BookDesk(DeskBookingRequest);
        }
    }
}