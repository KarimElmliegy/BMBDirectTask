let messageTimer;

export const elements = {
  authView: document.querySelector("#auth-view"),
  appView: document.querySelector("#app-view"),
  message: document.querySelector("#message"),
  appNavigation: document.querySelector("#app-navigation"),
  homeLink: document.querySelector("#home-link"),
  myOrdersLink: document.querySelector("#my-orders-link"),
  loginButton: document.querySelector("#login-button"),
  logoutButton: document.querySelector("#logout-button"),
  welcomeName: document.querySelector("#welcome-name"),
  orderBuilder: document.querySelector("#order-builder"),
  workspaceGrid: document.querySelector(".workspace-grid"),
  homePage: document.querySelector("#home-page"),
  ordersPage: document.querySelector("#my-orders"),
  productsList: document.querySelector("#products-list"),
  productsPagination: document.querySelector("#products-pagination"),
  productsPrevious: document.querySelector("#products-previous"),
  productsNext: document.querySelector("#products-next"),
  cartList: document.querySelector("#cart-list"),
  cartTotal: document.querySelector("#cart-total"),
  cartCount: document.querySelector("#cart-count"),
  orderDescription: document.querySelector("#order-description"),
  orderForm: document.querySelector("#order-form"),
  ordersList: document.querySelector("#orders-list"),
  refreshButton: document.querySelector("#refresh-button"),
  deleteModal: document.querySelector("#delete-modal"),
  cancelDeleteButton: document.querySelector("#cancel-delete-button"),
  confirmDeleteButton: document.querySelector("#confirm-delete-button")
};

export function money(value) {
  return new Intl.NumberFormat("en-US", { style: "currency", currency: "USD" }).format(value);
}

export function escapeHtml(value) {
  const node = document.createElement("span");
  node.textContent = value ?? "";
  return node.innerHTML;
}

export function showMessage(text, isError = false) {
  clearMessage();
  elements.message.textContent = text;
  elements.message.hidden = false;
  elements.message.classList.toggle("is-error", isError);
  requestAnimationFrame(() => elements.message.classList.add("is-visible"));
  messageTimer = setTimeout(clearMessage, 2200);
}

export function clearMessage() {
  clearTimeout(messageTimer);
  elements.message.classList.remove("is-visible");
  window.setTimeout(() => {
    if (!elements.message.classList.contains("is-visible")) {
      elements.message.hidden = true;
      elements.message.textContent = "";
    }
  }, 220);
}

export function setAuthMode(mode) {
  document.querySelectorAll("[data-auth-tab]").forEach(button => {
    const selected = button.dataset.authTab === mode;
    button.classList.toggle("is-active", selected);
    button.setAttribute("aria-selected", String(selected));
  });
  document.querySelector("#login-form").hidden = mode !== "login";
  document.querySelector("#register-form").hidden = mode !== "register";
  clearMessage();
}

export function scrollToTop() {
  window.scrollTo({ top: 0, behavior: "smooth" });
}
