import ws from "k6/ws";
import http from "k6/http";
import { check, sleep } from "k6";
import { login, register } from "./utils/users.js";

export function seedTestUsers(baseUrl, totalUsers = 1000) {
  const userPayload = login(totalUsers);

  console.log(
    `[SETUP] Verifying pre-seeded account user_${totalUsers}...`,
  );

  const res = http.post(
    `${baseUrl}/api/v1/auth/login`,
    JSON.stringify(userPayload),
    { headers: { "Content-Type": "application/json" } },
  );

  const ok = check(res, {
    "Pre-seeded user login verified (200 OK)": (r) => r.status === 200,
  });

  if (!ok) {
    console.log(
      `[SETUP] Seeding ${totalUsers} test users against ${baseUrl}...`,
    );
    for (let i = 1; i <= totalUsers; i++) {
      const userPayload = JSON.stringify(register(i));

      http.post(`${baseUrl}/api/v1/auth/register`, userPayload, {
        headers: { "Content-Type": "application/json" },
      });
    }
    console.log(`[SETUP] Finished seeding ${totalUsers} users.`);
  }
}

// 2. VU SCENARIO: Executed by each Virtual User thread during the test run[cite: 1]
export function simulateChatUser(baseUrl) {
  const headers = { "Content-Type": "application/json" };
  const user = login(__VU);
  const loginRes = http.post(
    `${baseUrl}/api/v1/auth/login`,
    JSON.stringify(user),
    { headers },
  );

  const authenticated = check(loginRes, {
    "Login successful (200 OK)": (r) => r.status === 200,
  });

  if (!authenticated) {
    console.error(`VU ${__VU} failed login (${loginRes.status})`);
    return;
  }

  // B. Establish SignalR WebSocket connection using the authenticated cookie
  const wsUrl = baseUrl.replace("http", "ws") + "/hubs/v1/chat";

  const res = ws.connect(wsUrl, {}, function (socket) {
    socket.on("open", () => {
      // SignalR JSON protocol handshake (terminated by 0x1E ASCII record separator)[cite: 1]
      socket.send('{"protocol":"json","version":1}\x1e');
    });

    socket.on("message", (data) => {
      check(data, {
        "Received SignalR frame": (msg) => msg.length > 0,
      });
    });

    // C. Perform REST requests while keeping the WebSocket connection active
    const httpRes = http.get(`${baseUrl}/api/v1/chats`);
    check(httpRes, {
      "GET /chats status is 200": (r) => r.status === 200,
    });

    socket.setTimeout(() => {
      socket.close();
    }, 30000);
  });

  check(res, {
    "WebSocket connected successfully": (r) => r && r.status === 101,
  });

  sleep(1);
}
