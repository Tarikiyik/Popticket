document.querySelectorAll(".seat").forEach((seat) => {
  seat.addEventListener("click", () => {
    if (!seat.classList.contains("occupied")) {
      seat.classList.toggle("selected");
    }
  });
});
