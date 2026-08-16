import { state, clearSession, saveSession } from "./state.js";
import { loadProducts, initCatalog, renderProducts } from "./catalog.js";
import { addToCart, initCart, renderCart, syncCartInventory } from "./cart.js";
import { initOrders, loadOrders, renderOrders } from "./orders.js";
import { initAuth } from "./auth.js";
import { elements, scrollToTop, setAuthMode, showMessage } from "./ui.js";

function showLogin() {
  setAuthMode("login");
  elements.authView.hidden = false;
  elements.appView.hidden = true;
  scrollToTop();
}

function isBanned() {
  return Boolean(state.session?.bannedUntil && new Date(state.session.bannedUntil) > new Date());
}

function showBanMessage() {
  const bannedUntil = new Date(state.session.bannedUntil).toLocaleString();
  showMessage(`Your account is temporarily restricted until ${bannedUntil}. You can browse products, but you cannot create orders.`, true);
}

function showPage(page, updateHistory = true) {
  if (page === "orders" && !state.session?.token) {
    showLogin();
    showMessage("Log in to view your orders.");
    return;
  }
  const showOrders = page === "orders";
  elements.authView.hidden = true;
  elements.appView.hidden = false;
  elements.homePage.hidden = showOrders;
  elements.ordersPage.hidden = !showOrders;
  elements.homeLink.hidden = !state.session?.token;
  elements.myOrdersLink.hidden = !state.session?.token;
  document.querySelectorAll("[data-app-page]").forEach(link => link.classList.toggle("is-active", link.dataset.appPage === page));
  if (updateHistory) history.pushState(null, "", showOrders ? "#my-orders" : "#home");
}

function updateSessionView() {
  const authenticated = Boolean(state.session?.token);
  elements.authView.hidden = true;
  elements.appView.hidden = false;
  elements.appNavigation.hidden = !authenticated;
  elements.loginButton.hidden = authenticated;
  elements.logoutButton.hidden = !authenticated;
  const banned = isBanned();
  elements.orderBuilder.hidden = !authenticated || banned;
  elements.workspaceGrid.classList.toggle("is-public", !authenticated || banned);
  elements.welcomeName.textContent = authenticated ? state.session.name : "";
  showPage(authenticated && location.hash === "#my-orders" ? "orders" : "home", false);
}

function handleError(error) {
  if (/401|token|Unauthorized/i.test(error.message)) {
    clearSession();
    updateSessionView();
  }
  showMessage(error.message, true);
}

function refreshWorkspace() {
  return Promise.all([loadProducts(), state.session?.token ? loadOrders() : Promise.resolve()])
    .then(() => syncCartInventory())
    .catch(handleError);
}

initCatalog({
  onAddVariant: variantId => {
    if (!state.session?.token) {
      state.pendingVariantId = variantId;
      showLogin();
      showMessage("Log in to add this phone to your order.");
      return;
    }
    if (isBanned()) {
      showBanMessage();
      return;
    }
    addToCart(variantId);
  }
});
initCart({ onOrderCreated: order => { state.orders.unshift(order); renderOrders(); refreshWorkspace(); }, onError: handleError });
initOrders({
  onError: handleError,
  onOrderDeleted: refreshWorkspace,
  onBanChanged: bannedUntil => {
    saveSession({ ...state.session, bannedUntil });
    state.cart = [];
    renderCart();
    renderProducts();
    updateSessionView();
    showBanMessage();
  }
});
initAuth({
  onLoginRequired: showLogin,
  onAuthenticated: () => {
    updateSessionView();
    refreshWorkspace();
    if (state.pendingVariantId) {
      const pendingVariantId = state.pendingVariantId;
      state.pendingVariantId = null;
      addToCart(pendingVariantId);
    }
  }
});

elements.logoutButton.addEventListener("click", () => {
  clearSession();
  state.cart = [];
  state.orders = [];
  renderCart();
  renderOrders();
  updateSessionView();
});
document.querySelectorAll("[data-app-page]").forEach(link => link.addEventListener("click", event => { event.preventDefault(); showPage(link.dataset.appPage); }));
window.addEventListener("popstate", () => showPage(location.hash === "#my-orders" ? "orders" : "home", false));
window.addEventListener("hashchange", () => showPage(location.hash === "#my-orders" ? "orders" : "home", false));
elements.refreshButton.addEventListener("click", event => {
  const button = event.currentTarget;
  button.disabled = true;
  refreshWorkspace().finally(() => { button.disabled = false; });
});

renderCart();
updateSessionView();
refreshWorkspace();
