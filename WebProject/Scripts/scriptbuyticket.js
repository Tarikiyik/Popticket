document.addEventListener("DOMContentLoaded", function () {
    let selectedTheater = null;
    let selectedDate = null;
    let selectedTime = null;
    let ticketSelected = false;
    let selectedTickets = {};
    let movieId = document.querySelector('.buy-ticket-page').dataset.movieId;

    // Call this function on initial load so the theaters are displayed before any city is selected
    fetchTheaters(null);

    // Function to clear selections
    function clearSelections(selector) {
        document.querySelectorAll(selector).forEach(el => el.classList.remove('selected'));
    }

    // Clear date and time selections
    function clearTimeSelection() {
        let timeSelectionDiv = document.getElementById("time-selection");
        while (timeSelectionDiv.firstChild) {
            timeSelectionDiv.removeChild(timeSelectionDiv.firstChild);
        }
    }

    function clearDateAndTimeSelection() {
        let dateSelectionDiv = document.getElementById("date-selection");
        while (dateSelectionDiv.firstChild) {
            dateSelectionDiv.removeChild(dateSelectionDiv.firstChild);
        }
        clearTimeSelection();
    }

    // Function to update total price and handle ticket selection
    function updateTotalPrice() {
        let total = 0;
        ticketSelected = false; // Flag to detect if any ticket quantity is more than 0

        for (const ticket of Object.values(selectedTickets)) {
            total += (ticket.quantity * ticket.price);
            if (ticket.quantity > 0) ticketSelected = true; // At least one ticket is selected
        }

        document.getElementById("ticket-total-price").textContent = `Total Price: ${total.toFixed(2)}TL`;

        // Call the function to enable or disable the continue button based on ticket selection
        updateContinueButtonState(ticketSelected);
    }

    // Function to enable/disable the continue button
    function updateContinueButtonState() {
        const continueButton = document.getElementById('ticket-confirmation-button');
        continueButton.disabled = !(selectedTheater && selectedDate && selectedTime && ticketSelected);
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

    // Add event delegation for dynamically created date elements
    document.getElementById("date-selection").addEventListener("click", function (e) {
        if (e.target.classList.contains('date-container')) {
            // Toggle selection if the same date is clicked again
            if (e.target.classList.contains('selected')) {
                e.target.classList.remove('selected');
                selectedDate = null;
            } else {
                clearSelections("#date-selection .date-container.selected");
                e.target.classList.add('selected');
                selectedDate = e.target.dataset.date;
            }
            // Clear times when date selection changes
            clearSelections("#time-selection .time-container.selected");
            selectedTime = null;
            updateContinueButtonState();
            if (selectedDate) {
                fetchTimes(selectedDate);
            } else {
                clearTimeSelection();
            }
        }
    });

    // Add event delegation for dynamically created time elements
    document.getElementById("time-selection").addEventListener("click", function (e) {
        if (e.target.classList.contains('time-container')) {
            // Toggle selection if the same time is clicked again
            if (e.target.classList.contains('selected')) {
                e.target.classList.remove('selected');
                selectedTime = null;
            } else {
                clearSelections("#time-selection .time-container.selected");
                e.target.classList.add('selected');
                selectedTime = e.target.dataset.time;
            }
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
        }
    });

    document.getElementById("ticket-confirmation-button").addEventListener("click", function () {
        let totalQuantity = 0;
        let totalPrice = 0;

        Object.values(selectedTickets).forEach(ticket => {
            totalQuantity += ticket.quantity;
            totalPrice += ticket.quantity * ticket.price;
        });

        let dataToSend = {
            theaterId: selectedTheater,
            date: selectedDate,
            time: selectedTime,
            totalQuantity: totalQuantity,
            totalPrice: totalPrice
        };

        $.ajax({
            url: '/BuyTicket/SelectSeats', // Make sure this endpoint matches your server configuration
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(dataToSend),
            dataType: 'json',
            success: function (response) {
                if (response.redirectUrl) {
                    window.location.href = response.redirectUrl; // Redirect to SelectSeat page
                } else {
                    console.error("No redirect URL provided in response.");
                }
            },
            error: function (xhr, status, error) {
                console.error("Error occurred: ", status, error);
                // Optionally, display an error message to the user
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
        // Event listeners for theater selection
        document.querySelectorAll(".theater-select-container").forEach(container => {
            container.addEventListener("click", function () {
                // Toggle selection if the same theater is clicked again
                if (this.classList.contains('selected')) {
                    this.classList.remove('selected');
                    selectedTheater = null;
                } else {
                    clearSelections(".theater-select-container.selected");
                    this.classList.add('selected');
                    selectedTheater = this.dataset.theaterId;
                }
                // Clear dates and times when theater selection changes
                clearSelections("#date-selection .date-container.selected");
                clearSelections("#time-selection .time-container.selected");
                selectedDate = null;
                selectedTime = null;
                updateContinueButtonState();
                if (selectedTheater) {
                    fetchDates(selectedTheater);
                } else {
                    clearDateAndTimeSelection();
                }
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
