using DeskBooker.Core.DataInterface;
using DeskBooker.Core.Domain;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeskBooker.Core.Processor
{
    [TestFixture]
    public class DeskBookingRequestProcessorTests
    {
        private DeskBookingRequestProcessor _processor;
        private DeskBookingRequest _deskBookingRequest;
        private Mock<IDeskBookingRepository> _deskBookingRepositoryMock;
        private Mock<IDeskRepository> _deskRepositoryMock;
        private List<Desk> _availableDesks;

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

            _deskRepositoryMock = new Mock<IDeskRepository>();

            _availableDesks = new List<Desk> { new Desk { Id = 7 } };

            _processor = new DeskBookingRequestProcessor(_deskBookingRepositoryMock.Object, _deskRepositoryMock.Object);

            _deskRepositoryMock.Setup(x => x.GetAvailableDesks(_deskBookingRequest.Date)).Returns(_availableDesks);

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
            Assert.AreEqual(_availableDesks.First().Id, savedDeskBooking.DeskId);
        }
        [Test]
        public void ShouldNotSaveDeskBookingIfNoDeskAvailable()
        {
            // ARRANGE so no desk is available. 
            _availableDesks.Clear();

            _processor.BookDesk(_deskBookingRequest);
            _deskBookingRepositoryMock.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Never);
        }
        //Data driven test
        [TestCase(DeskBookingResultCode.Success, true)]
        [TestCase(DeskBookingResultCode.NoDeskAvailable, false)]
        public void ShouldReturnExpectedResultCode(DeskBookingResultCode expectedResultCode, bool isDeskAvailable)
        {
            // Arrange that if the variable is false make sure the list of desks is empty
            if (!isDeskAvailable)
            {
                _availableDesks.Clear();
            }

            var result = _processor.BookDesk(_deskBookingRequest);
            Assert.AreEqual(expectedResultCode, result.Code);

        }


        [TestCase(5, true)]
        [TestCase(null, false)]
        public void ShouldReturnExpectedDeskBookingId(
     int? expectedDeskBookingId, bool isDeskAvailable)
        {
            if (!isDeskAvailable)
            {
                _availableDesks.Clear();
            }
            else
            {
                _deskBookingRepositoryMock.Setup(x => x.Save(It.IsAny<DeskBooking>()))
                  .Callback<DeskBooking>(deskBooking =>
                  {
                      deskBooking.Id = expectedDeskBookingId.Value;
                  });
            }

            var result = _processor.BookDesk(_deskBookingRequest);

            Assert.AreEqual(expectedDeskBookingId, result.DeskBookingId);
        }
    }
}
