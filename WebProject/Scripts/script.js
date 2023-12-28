var counter = 1;
var interval;

function setRadioInterval() {
  interval = setInterval(function () {
    document.getElementById("radio" + counter).checked = true;
    counter++;
    if (counter > 3) {
      counter = 1;
    }
  }, 5000);
}

setRadioInterval();
document
  .querySelectorAll(".navigation-manual .manual-btn")
  .forEach(function (btn, index) {
    btn.addEventListener("click", function () {
      clearInterval(interval);
      counter = index + 1;
      setTimeout(setRadioInterval, 5000);
    });
  });

document.addEventListener("DOMContentLoaded", function () {
  setupCarousel(
    "carousel_cont",
    "carousel_previous_button",
    "carousel_next_button"
  );
  setupCarousel(
    "upcoming_carousel_cont",
    "upcoming_carousel_previous_button",
    "upcoming_carousel_next_button"
  );
});

function setupCarousel(containerId, prevButtonId, nextButtonId) {
  const container = document.getElementById(containerId);
  const prevButton = document.getElementById(prevButtonId);
  const nextButton = document.getElementById(nextButtonId);
  let currentTransform = 0;

  prevButton.addEventListener("click", function () {
    currentTransform = Math.min(currentTransform + 330, 0); // To prevent scrolling too far left
    container.style.transform = `translateX(${currentTransform}px)`;
  });

  nextButton.addEventListener("click", function () {
    const maxTransform = -(container.scrollWidth - container.clientWidth); // To prevent scrolling too far right
    currentTransform = Math.max(currentTransform - 330, maxTransform);
    container.style.transform = `translateX(${currentTransform}px)`;
  });
}

