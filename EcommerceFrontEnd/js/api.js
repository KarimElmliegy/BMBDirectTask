import { state } from "./state.js";

// Use the same host in production while preserving the existing local API workflow.
const API_BASE_URL = window.location.port === "5500"
  ? "https://localhost:7275/api"
  : "/api";

export function api(path, options = {}) {
  const headers = { "Content-Type": "application/json", ...(options.headers || {}) };
  if (state.session?.token) headers.Authorization = `Bearer ${state.session.token}`;

  return fetch(`${API_BASE_URL}${path}`, { ...options, headers })
    .then(response => response.status === 204 ? null : response.json().then(body => ({ response, body })))
    .then(result => {
      if (result === null) return null;
      if (!result.response.ok) return Promise.reject(new Error(result.body.detail || result.body.title || "The request failed."));
      return result.body;
    });
}
