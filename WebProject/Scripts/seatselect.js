document.addEventListener("DOMContentLoaded", function () {
    const selectedSeats = new Set();
    const maxSeats = window.showtimeData.ticketQuantity;

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

    // Continue to Payment Page (Placeholder Function)
    document.getElementById('continue-button').addEventListener('click', () => {
        // Logic to handle transition to the payment page
        // You will need to pass selectedSeats and other relevant data
        console.log(Array.from(selectedSeats)); // For testing
    });
});