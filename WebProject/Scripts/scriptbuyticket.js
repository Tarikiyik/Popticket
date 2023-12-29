document.addEventListener("DOMContentLoaded", function () {
    let selectedTheater = null;
    let selectedDate = null;
    let selectedTime = null;
    let selectedTickets = {};
    let movieId = document.querySelector('.buy-ticket-page').dataset.movieId;

    // Call this function on initial load so the theaters are displayed before any city is selected
    fetchTheaters(null);

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
            success: function (response) {
                // Clear the date containers
                let dateSelectionDiv = document.getElementById("date-selection");
                while (dateSelectionDiv.firstChild) {
                    dateSelectionDiv.removeChild(dateSelectionDiv.firstChild);
                }
                // Clear the time containers
                let timeSelectionDiv = document.getElementById("time-selection");
                while (timeSelectionDiv.firstChild) {
                    timeSelectionDiv.removeChild(timeSelectionDiv.firstChild);
                }
                // Append new date containers if the request was successful
                if (response.success) {
                    response.availableDates.forEach(date => {
                        let dateDiv = document.createElement('div');
                        dateDiv.className = 'date-container';
                        dateDiv.dataset.date = date;
                        dateDiv.textContent = formatDate(date); // No need to format date as it's already in the desired format
                        dateSelectionDiv.appendChild(dateDiv);
                    });
                } else {
                    // Handle error if success is false
                    console.error("Error fetching dates: ", response.error);
                }
            },
            error: function (xhr, status, error) {
                console.error("Error fetching dates: ", status, error);
            }
        });
    }

    // Update the fetchTimes function to handle the new success property
    function fetchTimes(date) {
        $.ajax({
            url: '/BuyTicket/GetTimes',
            type: 'GET',
            data: { date: date, theaterId: selectedTheater, movieId: movieId },
            success: function (response) {
                if (response.success) {
                    let timeSelectionDiv = document.getElementById("time-selection");
                    // Clear the container safely
                    while (timeSelectionDiv.firstChild) {
                        timeSelectionDiv.removeChild(timeSelectionDiv.firstChild);
                    }
                    // Append new time containers
                    response.availableTimes.forEach(time => {
                        let timeDiv = document.createElement('div');
                        timeDiv.className = 'time-container';
                        timeDiv.dataset.time = time;
                        timeDiv.textContent = time; // Directly using the time string
                        timeSelectionDiv.appendChild(timeDiv);
                    });
                } else {
                    console.error("Error fetching times: ", response.error);
                }
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

    // Add event delegation for dynamically created date elements
    document.getElementById("date-selection").addEventListener("click", function (e) {
        if (e.target.classList.contains('date-container')) {
            clearSelections("#date-selection .date-container.selected");
            clearSelections("#time-selection .time-container.selected");
            e.target.classList.add('selected');
            selectedDate = e.target.dataset.date;
            fetchTimes(selectedDate);
            selectedTime = null; // Reset selected time
            document.getElementById("time-selection").textContent = ''; // Clear time selection
            updateContinueButtonState();
        }
    });

    // Add event delegation for dynamically created time elements
    document.getElementById("time-selection").addEventListener("click", function (e) {
        if (e.target.classList.contains('time-container')) {
            // Remove the 'selected' class from all time containers
            document.querySelectorAll("#time-selection .time-container").forEach(timeDiv => {
                timeDiv.classList.remove('selected');
            });
            // Add the 'selected' class to the clicked time container
            e.target.classList.add('selected');
            selectedTime = e.target.dataset.time;
            updateContinueButtonState();
        }
    });

    // Event listeners for ticket count buttons
    document.querySelector(".buy-info-ticket-options").addEventListener("click", function (e) {
        if (e.target.classList.contains('buy-info-ticket-count-button')) {
            const button = e.target;
            const container = button.closest('.buy-info-ticket-container');
            const ticketType = container.dataset.ticketTypeId;
            const price = parseFloat(container.querySelector(".buy-info-ticket-pricing h3").textContent.replace('TL', ''));
            const ticketCountValueElement = container.querySelector(".ticket-count-value");
            let quantity = parseInt(ticketCountValueElement.textContent);

            if (button.classList.contains('increase')) {
                quantity++;
            } else if (button.classList.contains('reduce') && quantity > 0) {
                quantity--;
            }

            selectedTickets[ticketType] = { quantity: quantity, price: price };
            ticketCountValueElement.textContent = quantity;
            updateTotalPrice();
            updateContinueButtonState();
        }
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

    // Event listener for the city dropdown change
    document.getElementById('city-dropdown').addEventListener('change', function () {
        clearSelections(".theater-select-container.selected");
        clearSelections("#date-selection .date-container.selected");
        clearSelections("#time-selection .time-container.selected");
        const selectedCityId = this.value;
        fetchTheaters(selectedCityId);
        selectedTheater = null;
        selectedDate = null;
        selectedTime = null;
        updateContinueButtonState();
    });

    function fetchTheaters(cityId) {
        $.ajax({
            url: '/BuyTicket/GetTheatersByCity',
            type: 'GET',
            data: { cityId: cityId }, // Pass the selected cityId to the controller
            success: function (response) {
                if (response.success) {
                    let dateSelectionDiv = document.getElementById("date-selection");
                    while (dateSelectionDiv.firstChild) {
                        dateSelectionDiv.removeChild(dateSelectionDiv.firstChild);
                    }

                    let timeSelectionDiv = document.getElementById("time-selection");
                    while (timeSelectionDiv.firstChild) {
                        timeSelectionDiv.removeChild(timeSelectionDiv.firstChild);
                    }
                    updateTheaterSelection(response.theaters);
                } else {
                    console.error("Error fetching theaters: ", response.error);
                }
            },
            error: function (xhr, status, error) {
                console.error("Error fetching theaters: ", status, error);
            }
        });
    }

    function updateTheaterSelection(theaters) {
        const theaterSelectionDiv = document.getElementById("theater-selection");
        // Clear the container safely
        while (theaterSelectionDiv.firstChild) {
            theaterSelectionDiv.removeChild(theaterSelectionDiv.firstChild);
        }
        // Append new theater containers
        theaters.forEach(theater => {
            const theaterDiv = document.createElement('div');
            theaterDiv.className = 'theater-select-container';
            theaterDiv.dataset.theaterId = theater.theaterID;
            theaterDiv.dataset.cityId = theater.cityID;
            theaterDiv.innerHTML = `<h4>${theater.name}</h4><p>${theater.address}</p>`;
            theaterSelectionDiv.appendChild(theaterDiv);
        });

        // Reattach event listeners to the new theater elements
        attachTheaterSelectionEvents();
    }

    function attachTheaterSelectionEvents() {
        // Attach click event listeners to all theater-select-container divs
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
    }

    function formatDate(dateString) {
        var date = new Date(dateString);
        var day = date.getDate();
        var month = date.toLocaleString('default', { month: 'long' });

        return `${day} ${month}`; // e.g., "29 December"
    }

});
