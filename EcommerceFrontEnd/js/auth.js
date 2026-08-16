import { api } from "./api.js";
import { saveSession } from "./state.js";
import { elements, setAuthMode, showMessage } from "./ui.js";

export function initAuth({ onLoginRequired, onAuthenticated }) {
  document.querySelectorAll("[data-auth-tab]").forEach(tab => tab.addEventListener("click", () => setAuthMode(tab.dataset.authTab)));

  elements.loginButton.addEventListener("click", onLoginRequired);

  document.querySelectorAll("#login-form, #register-form").forEach(form => form.addEventListener("submit", event => {
    event.preventDefault();
    const button = event.currentTarget.querySelector('[type="submit"]');
    const buttonLabel = button.textContent;
    const endpoint = event.currentTarget.id === "login-form" ? "/auth/login" : "/auth/register";
    button.disabled = true;
    button.classList.add("is-loading");
    button.textContent = endpoint === "/auth/login" ? "Logging in..." : "Creating account...";

    api(endpoint, { method: "POST", body: JSON.stringify(Object.fromEntries(new FormData(event.currentTarget))) })
      .then(session => { saveSession(session); onAuthenticated(); })
      .catch(error => showMessage(error.message, true))
      .finally(() => { button.disabled = false; button.classList.remove("is-loading"); button.textContent = buttonLabel; });
  }));
}
