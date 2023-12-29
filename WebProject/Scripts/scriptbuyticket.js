document.addEventListener("DOMContentLoaded", function () {
    let selectedTheater = null;
    let selectedDate = null;
    let selectedTime = null;
    let selectedTickets = {};
    let movieId = document.querySelector('.buy-ticket-page').dataset.movieId;

    // Function to clear selections
    function clearSelections(selector) {
        document.querySelectorAll(selector).forEach(el => el.classList.remove('selected'));
    }

    // Function to update total price
    function updateTotalPrice() {
        let total = Object.values(selectedTickets).reduce((sum, ticket) => {
            return sum + (ticket.quantity * ticket.price);
        }, 0);
        document.getElementById("ticket-total-price").textContent = `Total Price: ${total.toFixed(2)}TL`;
    }

    // Function to enable/disable the continue button
    function updateContinueButtonState() {
        const continueButton = document.getElementById('ticket-confirmation-button');
        continueButton.disabled = !(selectedTheater && selectedDate && selectedTime && Object.keys(selectedTickets).length > 0);
    }

    // Function to fetch dates based on selected theater
    function fetchDates(theaterId) {
        $.ajax({
            url: '/BuyTicket/GetDates',
            type: 'GET',
            data: { theaterId: theaterId, movieId: movieId },
            success: function (dates) {
                let dateSelectionDiv = document.getElementById("date-selection");
                dateSelectionDiv.innerHTML = '';
                dates.forEach(date => {
                    let dateDiv = document.createElement('div');
                    dateDiv.className = 'date-container';
                    dateDiv.dataset.date = date;
                    dateDiv.textContent = formatDate(date); // Implement formatDate to format the date as needed
                    dateSelectionDiv.appendChild(dateDiv);
                });
            },
            error: function (xhr, status, error) {
                console.error("Error fetching dates: ", status, error);
            }
        });
    }

    // Function to fetch times based on selected date and theater
    function fetchTimes(date) {
        $.ajax({
            url: '/BuyTicket/GetTimes',
            type: 'GET',
            data: { date: date, theaterId: selectedTheater, movieId: movieId },
            success: function (times) {
                let timeSelectionDiv = document.getElementById("time-selection");
                timeSelectionDiv.innerHTML = '';
                times.forEach(time => {
                    let timeDiv = document.createElement('div');
                    timeDiv.className = 'time-container';
                    timeDiv.dataset.time = time;
                    timeDiv.textContent = formatTime(time); // Implement formatTime to format the time as needed
                    timeSelectionDiv.appendChild(timeDiv);
                });
            },
            error: function (xhr, status, error) {
                console.error("Error fetching times: ", status, error);
            }
        });
    }

    // Event listeners for theater selection
    document.querySelectorAll(".theater-select-container").forEach(container => {
        container.addEventListener("click", function () {
            clearSelections(".theater-select-container.selected");
            clearSelections("#date-selection .date-container.selected");
            clearSelections("#time-selection .time-container.selected");
            this.classList.add('selected');
            selectedTheater = this.dataset.theaterId;
            fetchDates(selectedTheater);
            selectedDate = null;
            selectedTime = null;
            updateContinueButtonState();
        });
    });

    // Event listener for date selection
    document.getElementById("date-selection").addEventListener("click", function (e) {
        if (e.target.classList.contains('date-container')) {
            clearSelections("#date-selection .date-container.selected");
            clearSelections("#time-selection .time-container.selected");
            e.target.classList.add('selected');
            selectedDate = e.target.dataset.date;
            fetchTimes(selectedDate);
            selectedTime = null;
            updateContinueButtonState();
        }
    });

    // Event listeners for time selection
    document.querySelectorAll("#time-selection").addEventListener("click", function (e) {
        if (e.target.classList.contains('time-container')) {
            clearSelections("#time-selection .time-container.selected");
            e.target.classList.add('selected');
            selectedTime = e.target.dataset.time;
            updateContinueButtonState();
        }
    });

    // Event listeners for ticket count buttons
    document.querySelectorAll(".buy-info-ticket-container").forEach(container => {
        let ticketType = container.dataset.ticketTypeId;
        let price = parseFloat(container.querySelector(".buy-info-ticket-pricing h3").textContent.replace('TL', ''));
        container.querySelectorAll(".buy-info-ticket-count-button").forEach(button => {
            button.addEventListener("click", function () {
                let action = this.dataset.action;
                selectedTickets[ticketType] = selectedTickets[ticketType] || { quantity: 0, price: price };

                if (action === 'increase') {
                    selectedTickets[ticketType].quantity++;
                } else if (action === 'reduce' && selectedTickets[ticketType].quantity > 0) {
                    selectedTickets[ticketType].quantity--;
                }

                container.querySelector(".ticket-count-value").textContent = selectedTickets[ticketType].quantity;
                updateTotalPrice();
                updateContinueButtonState();
            });
        });
    });

    // Event listener for the continue button
    document.getElementById("ticket-confirmation-button").addEventListener("click", function () {
        let dataToSend = {
            theaterId: selectedTheater,
            date: selectedDate,
            time: selectedTime,
            tickets: Object.keys(selectedTickets).map(key => ({
                ticketType: key,
                quantity: selectedTickets[key].quantity
            }))
        };

        // AJAX request to server
        $.ajax({
            url: '/BuyTicket/ConfirmTicketSelection', // Replace with your actual endpoint
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(dataToSend),
            dataType: 'json',
            success: function (response) {
                window.location.href = response.redirectUrl;
            },
            error: function (xhr, status, error) {
                console.error("Error occurred: ", status, error);
            }
        });
    });

    // Utility function to format date
    function formatDate(dateString) {
        var date = new Date(dateString);
        var options = { year: 'numeric', month: 'long', day: 'numeric' };
        return date.toLocaleDateString('en-US', options);
    }

    // Utility function to format time
    function formatTime(timeString) {
        // Implement time formatting logic
        return timeString; // Placeholder
    }
});
