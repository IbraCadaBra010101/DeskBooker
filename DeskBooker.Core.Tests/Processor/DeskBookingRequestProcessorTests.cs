using DeskBooker.Core.DataInterface;
using DeskBooker.Core.Domain;
using Moq;
using NUnit.Framework;
using System;

namespace DeskBooker.Core.Processor
{
    [TestFixture]
    public class DeskBookingRequestProcessorTests
    {
        private DeskBookingRequestProcessor _processor;
        private DeskBookingRequest _deskBookingRequest;
        private Mock<IDeskBookingRepository> _deskBookingRepositoryMock;
        private Mock<IDeskRepository> _deskRepositoryMock;

        [SetUp]
        public void Setup()
        {
            // Arrange

            _deskBookingRequest = new DeskBookingRequest
            {
                FirstName = "Alan",
                LastName = "Shearer",
                Email = "AlanShearer@gmail.com",
                Date = new DateTime(2020, 1, 28)
            };

            _deskBookingRepositoryMock = new Mock<IDeskBookingRepository>();

            _processor = new DeskBookingRequestProcessor(_deskBookingRepositoryMock.Object);

            _deskRepositoryMock = new Mock<IDeskRepository>();

        }

        [Test]

        public void ShouldReturnDeskBookingResultWithRequestValues()
        {

            // Act
            DeskBookingResult result = _processor.BookDesk(_deskBookingRequest);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_deskBookingRequest.FirstName, result.FirstName);
            Assert.AreEqual(_deskBookingRequest.LastName, result.LastName);
            Assert.AreEqual(_deskBookingRequest.Email, result.Email);
            Assert.AreEqual(_deskBookingRequest.Date, result.Date);
        }

        [Test]
        public void ShouldThrowExceptionIfRequestIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => _processor.BookDesk(null));

            Assert.AreEqual("request", exception.ParamName);
        }

        [Test]

        public void ShouldSaveDeskBooking()
        {
            // Arrange
            DeskBooking savedDeskBooking = null;
            _deskBookingRepositoryMock.Setup(x => x.Save(It.IsAny<DeskBooking>()))
                .Callback<DeskBooking>(deskBooking =>
                {
                    savedDeskBooking = deskBooking;
                });

            _processor.BookDesk(_deskBookingRequest);

            // Act
            _deskBookingRepositoryMock.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Once);

            // Assert 
            Assert.NotNull(savedDeskBooking);
            // Assert properties
            Assert.AreEqual(_deskBookingRequest.FirstName, savedDeskBooking.FirstName);
            Assert.AreEqual(_deskBookingRequest.LastName, savedDeskBooking.LastName);
            Assert.AreEqual(_deskBookingRequest.Email, savedDeskBooking.Email);
            Assert.AreEqual(_deskBookingRequest.Date, savedDeskBooking.Date);
        }
        public void ShouldNotSaveDeskBookingIfNoDeskAvailable()
        {
            // ARRANGE so no desk is available. 
            _processor.BookDesk(_deskBookingRequest);
            _deskBookingRepositoryMock.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Never);
            _deskRepositoryMock.Setup(x => x.GetAvailableDesks(_deskBookingRequest.Date));


        }
    }
}
