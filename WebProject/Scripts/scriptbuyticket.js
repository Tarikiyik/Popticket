document.addEventListener("DOMContentLoaded", function () {
  // Scroll 100 pixels from the top, replace 100 with your desired scroll position
  window.scrollTo({ top: 300, behavior: "smooth" });
});

document.querySelectorAll(".custom-select-wrapper").forEach(function (wrapper) {
  const visiblePart = wrapper.querySelector(".custom-select-visible");
  visiblePart.addEventListener("click", function () {
    toggleCustomSelect(wrapper);
  });
});

function toggleCustomSelect(wrapper) {
  const optionsList = wrapper.querySelector(".custom-select-options");
  optionsList.classList.toggle("hidden");
}

document
  .querySelectorAll(".custom-select-options li")
  .forEach(function (option) {
    option.addEventListener("click", function () {
      const wrapper = this.closest(".custom-select-wrapper");
      const text = this.textContent;
      const value = this.getAttribute("data-value");
      const placeholder = document.getElementById("custom-placeholder");
      const nativeSelect = wrapper.querySelector(".custom-select");
      console.log("Option clicked:", text, value);
      // Update the placeholder text
      placeholder.textContent = text;

      // Update the value of the native select element
      nativeSelect.value = value;

      // Hide the custom options again
      wrapper.querySelector(".custom-select-options").classList.add("hidden");

      // Optional: Trigger any change event listeners attached to the native select
      const event = new Event("change", { bubbles: true });
      nativeSelect.dispatchEvent(event);
    });
  });

// Close the custom select options if clicking outside of the select
window.addEventListener("click", function (event) {
  if (!event.target.closest(".custom-select-wrapper")) {
    document
      .querySelectorAll(".custom-select-options")
      .forEach(function (optionsList) {
        optionsList.classList.add("hidden");
      });
  }
});

let select_theater = document.querySelectorAll(".theater-select-container");
let select_date = document.querySelectorAll(".date-container");
let select_time = document.querySelectorAll(".time-container");
let total_ticket_price = document.getElementById("ticket-total-price");
let ticket_reduce_standard = document.getElementById("ticket-reduce-count-sta");
let ticket_count_standard = document.getElementById(
  "buy-info-ticket-count-value-sta"
);
let ticket_increase_standard = document.getElementById(
  "ticket-increase-count-sta"
);
let ticket_reduce_student = document.getElementById("ticket-reduce-count-stu");
let ticket_count_student = document.getElementById(
  "buy-info-ticket-count-value-stu"
);
let ticket_increase_student = document.getElementById(
  "ticket-increase-count-stu"
);
let ticket_confirmation = document.getElementById("ticket-confirmation-button");
let ticket_price_standard = document.getElementById("ticket-price-standard");
let ticket_price_student = document.getElementById("ticket-price-student");

function clearSelections(elements) {
  elements.forEach((element) => {
    element.style.background = ""; // Reset background to default for all containers
    element.dataset.selected = "false";
  });
}

// Add click event listener to each theater container
select_theater.forEach((container) => {
  container.addEventListener("click", function () {
    if (this.dataset.selected === "true") {
      this.style.background = ""; // Deselect it by resetting the background
      this.dataset.selected = "false"; // Update the data-selected attribute to false
    } else {
      clearSelections(select_theater); // First clear all previous selections
      this.style.background = "rgb(255, 208, 0)"; // Set the background of the clicked container
      this.dataset.selected = "true"; // Update the data-selected attribute to true
    }
  });
});

select_time.forEach((container) => {
  container.addEventListener("click", function () {
    if (this.dataset.selected === "true") {
      this.style.background = ""; // Deselect it by resetting the background
      this.dataset.selected = "false"; // Update the data-selected attribute to false
    } else {
      clearSelections(select_time); // First clear all previous selections
      this.style.background = "rgb(255, 208, 0)"; // Set the background of the clicked container
      this.dataset.selected = "true"; // Update the data-selected attribute to true
    }
  });
});
select_date.forEach((container) => {
  container.addEventListener("click", function () {
    if (this.dataset.selected === "true") {
      this.style.background = ""; // Deselect it by resetting the background
      this.dataset.selected = "false"; // Update the data-selected attribute to false
    } else {
      clearSelections(select_date); // First clear all previous selections
      this.style.background = "rgb(255, 208, 0)"; // Set the background of the clicked container
      this.dataset.selected = "true"; // Update the data-selected attribute to true
    }
  });
});

ticket_reduce_standard.addEventListener("click", function () {
  if (ticket_count_standard.textContent > 0) {
    ticket_count_standard.textContent--;
    calculateTotal();
  }
});
ticket_increase_standard.addEventListener("click", function () {
  ticket_count_standard.textContent++;
  calculateTotal();
});
ticket_reduce_student.addEventListener("click", function () {
  if (ticket_count_student.textContent > 0) {
    ticket_count_student.textContent--;
    calculateTotal();
  }
});
ticket_increase_student.addEventListener("click", function () {
  ticket_count_student.textContent++;
  calculateTotal();
});

function calculateTotal() {
  let countStandard = ticket_count_standard.textContent;
  let countStudent = ticket_count_student.textContent;
  let studentPrice = parseInt(ticket_price_student.textContent, 10);
  let standardPrice = parseInt(ticket_price_standard.textContent, 10);
  let total = countStandard * standardPrice + countStudent * studentPrice;
  total_ticket_price.textContent = `Total Price: ${total.toFixed(2)}TL`;
}
calculateTotal();


