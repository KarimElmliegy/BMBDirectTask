import { api } from "./api.js";
import { state } from "./state.js";
import { elements, escapeHtml, money, showMessage } from "./ui.js";

const imageExtensions = ["jpg", "jpeg", "png", "webp"];

export function initOrders({ onError, onBanChanged, onOrderDeleted }) {
  let pendingDeleteId = null;
  let lastFocusedElement = null;

  const closeDeleteModal = () => {
    pendingDeleteId = null;
    elements.deleteModal.hidden = true;
    lastFocusedElement?.focus();
  };

  const deleteOrder = () => {
    if (!pendingDeleteId) return;
    const deleteId = pendingDeleteId;
    elements.confirmDeleteButton.disabled = true;
    api(`/orders/${deleteId}`, { method: "DELETE" }).then(result => {
      state.orders = state.orders.filter(order => order.id !== deleteId);
      if (state.expandedOrderId === deleteId) state.expandedOrderId = null;
      closeDeleteModal();
      renderOrders();
      onOrderDeleted();
      if (result.bannedUntil) onBanChanged(result.bannedUntil);
      else showMessage("Order deleted.");
    }).catch(onError).finally(() => { elements.confirmDeleteButton.disabled = false; });
  };

  elements.cancelDeleteButton.addEventListener("click", closeDeleteModal);
  elements.confirmDeleteButton.addEventListener("click", deleteOrder);
  elements.deleteModal.addEventListener("click", event => { if (event.target === elements.deleteModal) closeDeleteModal(); });
  document.addEventListener("keydown", event => { if (event.key === "Escape" && !elements.deleteModal.hidden) closeDeleteModal(); });

  elements.ordersList.addEventListener("click", event => {
    const deleteId = Number(event.target.dataset.deleteOrder);
    if (deleteId) {
      event.stopPropagation();
      pendingDeleteId = deleteId;
      lastFocusedElement = event.target;
      elements.deleteModal.hidden = false;
      elements.confirmDeleteButton.focus();
      return;
    }
    const orderCard = event.target.closest("[data-order-details]");
    if (!orderCard) return;
    const orderId = Number(orderCard.dataset.orderDetails);
    state.expandedOrderId = state.expandedOrderId === orderId ? null : orderId;
    renderOrders();
  });
  elements.ordersList.addEventListener("error", showNextProductImage, true);
}

export function loadOrders() { return api("/orders").then(orders => { state.orders = orders; renderOrders(); }); }

export function renderOrders() {
  if (!state.orders.length) { elements.ordersList.innerHTML = '<p class="empty">You have no active orders.</p>'; return; }
  elements.ordersList.innerHTML = state.orders.map(order => {
    const expanded = state.expandedOrderId === order.id;
    return `<article class="order${expanded ? " is-expanded" : ""}" data-order-details="${order.id}" tabindex="0"><div class="order-header"><div><h3>${escapeHtml(order.description)}</h3><p class="order-meta">Order #${order.id} · ${new Date(order.createdAt).toLocaleString()} · ${["Pending", "Completed", "Cancelled"][order.status] || "Unknown"}</p></div><div class="order-actions"><strong>${money(order.items.reduce((total, item) => total + item.unitPrice * item.quantity, 0))}</strong><button class="button button-danger" type="button" data-delete-order="${order.id}">Delete</button></div></div><p class="order-expand-hint">${expanded ? "Click this order to hide its products." : "Click this order to view its products."}</p>${expanded ? `<div class="ordered-products">${order.items.map(renderOrderItem).join("")}</div>` : ""}</article>`;
  }).join("");
}

function renderOrderItem(item) {
  const product = state.products.find(entry => entry.name === item.productName);
  const imageNumber = Math.max(1, state.products.findIndex(entry => entry.name === item.productName) + 1);
  return `<article class="order-product-card"><img class="order-product-image" src="Phonesimages/phone${imageNumber}.jpg" alt="${escapeHtml(item.productName)}" data-image-extension="0" data-image-number="${imageNumber}"><div><h4>${escapeHtml(item.productName)}</h4>${product?.description ? `<p class="order-product-description">${escapeHtml(product.description)}</p>` : ""}<dl class="phone-specifications"><div><dt>Color</dt><dd>${escapeHtml(item.color)}</dd></div><div><dt>Memory size</dt><dd>${escapeHtml(item.memorySize)}</dd></div><div><dt>Storage size</dt><dd>${escapeHtml(item.storageSize)}</dd></div>${item.otherDetails ? `<div><dt>Additional phone details</dt><dd>${escapeHtml(item.otherDetails)}</dd></div>` : ""}<div><dt>Quantity</dt><dd>${item.quantity}</dd></div><div><dt>Unit price</dt><dd>${money(item.unitPrice)}</dd></div></dl></div></article>`;
}

function showNextProductImage(event) {
  const image = event.target;
  const extensionIndex = Number(image.dataset.imageExtension) + 1;
  if (extensionIndex < imageExtensions.length) {
    image.dataset.imageExtension = extensionIndex;
    image.src = `Phonesimages/phone${image.dataset.imageNumber}.${imageExtensions[extensionIndex]}`;
  } else image.hidden = true;
}
