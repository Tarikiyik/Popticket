document.addEventListener("DOMContentLoaded", function () {
    const selectedSeats = new Set();
    const maxSeats = window.showtimeData.totalQuantity;
    const totalPrice = window.showtimeData.totalPrice;
    const continueButton = document.getElementById('continue-button');

    // Select or Deselect Seat
    function selectSeat(seatId) {
        const seatElement = document.querySelector(`[data-seat-id='${seatId}']`);
        if (seatElement.classList.contains('occupied')) {
            return; // Do nothing for occupied seats
        }

        if (seatElement.classList.contains('selected-seat')) {
            seatElement.classList.remove('selected-seat');
            selectedSeats.delete(seatId);
        } else if (selectedSeats.size < maxSeats) {
            seatElement.classList.add('selected-seat');
            selectedSeats.add(seatId);
        }

        // Enable or Disable Continue Button
        document.getElementById('continue-button').disabled = selectedSeats.size !== maxSeats;
    }

    // Mark Occupied Seats
    window.showtimeData.occupiedSeats.forEach(seatId => {
        const seatElement = document.querySelector(`[data-seat-id='${seatId}']`);
        if (seatElement) {
            seatElement.classList.add('occupied');
        }
    });

    // Add Event Listener to All Seats
    document.querySelectorAll('.seat').forEach(seat => {
        seat.addEventListener('click', () => selectSeat(seat.dataset.seatId));
    });

    // Continue Button Click Event
    continueButton.addEventListener('click', function () {
        if (selectedSeats.size === maxSeats) {
            const selectedSeatIds = Array.from(selectedSeats);
            const showtimeId = window.showtimeData.showtimeId;
            const theaterLayoutId = window.showtimeData.theaterLayoutId;
            const ticketQuantities = window.showtimeData.ticketQuantities;
            const ticketTypeIds = window.showtimeData.ticketTypeIds;

            // Prepare the AJAX call to send the selected seats and ticket types to the server
            $.ajax({
                url: '/BuyTicket/PrepareForPayment',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({
                    selectedSeatIds: selectedSeatIds,
                    showtimeId: showtimeId,
                    theaterLayoutId: theaterLayoutId,
                    totalPrice: totalPrice,
                    ticketQuantities: ticketQuantities,
                    ticketTypeIds: ticketTypeIds,
                }),
                success: function (response) {
                    if (response.success) {
                        window.location.href = response.redirectUrl; // Updated to use redirectUrl
                    } else {
                        alert('There was an error preparing for payment. Please try again.');
                    }
                },
                error: function (xhr, status, error) {
                    console.error('An error occurred while preparing for payment:', error);
                }
            });
        } else {
            alert('Please select the correct number of seats before continuing.');
        }
    });
});