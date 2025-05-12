function convertDates() {
    document.querySelectorAll(".local-date").forEach(el => {
        const utc = el.getAttribute("data-utc");
        if (!utc) return;
        const convertedDate = new Date(utc);
        el.textContent = convertedDate.toLocaleDateString(undefined, { dateStyle: 'medium' });
    });
}

document.addEventListener("DOMContentLoaded", () => {
    const updateBtn = document.getElementById("updateBtn");
    const container = document.getElementById("pricesContainer");
    const spinner = document.getElementById("spinner");
    const statusEl = document.getElementById("statusMessage");

    updateBtn.addEventListener("click", async () => {
        spinner.classList.remove("d-none");
        document.querySelectorAll("table.table").forEach(t => t.remove());

        const updResp = await fetch("/updatePrices", { method: "POST" });
        statusEl.innerText = updResp.ok
            ? ""
            : "❌ Failed to update.";

        const gridResp = await fetch("/cryptoInfoList", {
            headers: { "X-Requested-With": "XMLHttpRequest" }
        });
        if (gridResp.ok) {
            container.innerHTML = await gridResp.text();
            convertDates();
        } else {
            container.innerHTML = "<p class='text-danger'>Unable to load the data.</p>";
        }

        spinner.classList.add("d-none");
    });
});