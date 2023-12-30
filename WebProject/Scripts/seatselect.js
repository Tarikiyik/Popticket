document.addEventListener("DOMContentLoaded", function () {
    // Initialize variables to store data from the server
    var showtimeId;
    var occupiedSeats = [];
    var ticketQuantities = {};

    // Function to update the Continue button based on selected seats
    function updateContinueButton(selectedSeats) {
        var continueButton = document.querySelector(".select-seat-confirmation");
        var totalTickets = Object.values(ticketQuantities).reduce((acc, val) => acc + val, 0);
        continueButton.disabled = selectedSeats.size !== totalTickets;
    }

    // Function to mark a seat as occupied
    function markOccupiedSeats(occupiedSeats) {
        occupiedSeats.forEach(function (seatId) {
            var seatElement = document.querySelector(`.seat[data-seat-id="${seatId}"]`);
            if (seatElement) {
                seatElement.classList.add('occupied');
            }
        });
    }

    // Function to toggle seat selection
    function toggleSeatSelection(seat, selectedSeats) {
        if (seat.classList.contains("occupied")) {
            return; // Do nothing if the seat is already occupied
        }

        var seatId = seat.getAttribute('data-seat-id');
        if (seat.classList.contains("selected")) {
            selectedSeats.delete(seatId);
        } else {
            selectedSeats.add(seatId);
        }

        seat.classList.toggle("selected");
        updateContinueButton(selectedSeats); // Update the Continue button state
    }

    document.addEventListener("DOMContentLoaded", function () {
        var selectedSeats = new Set();

        // Fetch occupied seats from the server on page load
        fetchOccupiedSeats(showtimeId, function (data) {
            occupiedSeats = data;
            markOccupiedSeats(occupiedSeats);
        });

        // Event delegation for seat clicks
        document.querySelector(".seats").addEventListener("click", function (event) {
            if (event.target.classList.contains("seat")) {
                toggleSeatSelection(event.target, selectedSeats);
            }
        });

        // Fetch and display occupied seats from the server
        function fetchOccupiedSeats(showtimeId, callback) {
            var endpoint = '/BuyTicket/GetOccupiedSeats'; // Your actual endpoint URL
            $.ajax({
                url: endpoint,
                type: 'GET',
                data: { showtimeId: showtimeId },
                success: function (data) {
                    callback(data.occupiedSeats);
                },
                error: function (xhr, status, error) {
                    console.error('Error fetching occupied seats:', status, error);
                }
            });
        }

        // Initialize the page with data from the server
        function initializePage() {
            // Assuming 'showtimeData' is a global object containing data from the server
            showtimeId = showtimeData.showtimeId;
            ticketQuantities = showtimeData.ticketQuantities;
            fetchOccupiedSeats(showtimeId, function (occupied) {
                occupiedSeats = occupied;
                markOccupiedSeats(occupiedSeats);
            });
        }

        initializePage();
    });
});