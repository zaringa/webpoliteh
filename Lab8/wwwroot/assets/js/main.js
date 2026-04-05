(function () {
  function updateTime() {
    var clock = document.getElementById("clock");
    if (!clock) {
      return;
    }

    clock.textContent = "Client time: " + new Date().toLocaleString();
  }

  var btn = document.getElementById("refresh-time");
  if (btn) {
    btn.addEventListener("click", updateTime);
  }

  updateTime();
})();
