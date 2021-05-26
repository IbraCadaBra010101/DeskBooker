using DeskBooker.Core.Domain;
using DeskBooker.Core.Processor;
using Moq;
using NUnit.Framework;

namespace DeskBooker.Web.Pages
{
    [TestFixture]
    public class BookDeskModelTests
    {
        //[SetUp]

        //public void Setup()
        //{

        //}

        [Test]
        public void ShouldCallBookDeskMethodOfProcessor()
        {
            // Arrange
            var processorMock = new Mock<IDeskBookingRequestProcessor>();
            var bookDeskModel = new BookDeskModel(processorMock.Object)
            {
                DeskBookingRequest = new DeskBookingRequest()
            };
            // Act
            bookDeskModel.OnPost();

            // Assert
            processorMock.Verify(x => x.BookDesk(bookDeskModel.DeskBookingRequest), Times.Once);



        }
    }
}
