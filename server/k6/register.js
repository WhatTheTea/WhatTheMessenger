import http from 'k6/http';
import { check, sleep } from 'k6';
import { registerUnique } from './utils/users.js';

export default function registerScenario(baseUrl) {
  const url = `${baseUrl}/api/v1/auth/register`;
  const payload = JSON.stringify(registerUnique());

  const params = {
    headers: {
      'Content-Type': 'application/json',
      'Accept': 'application/json',
    },
  };

  // 2. Fire the HTTP POST request
  const res = http.post(url, payload, params);

  // 3. Validate response
  check(res, {
    'ok': (r) => r.status === 200 || r.status === 201,
  });

  sleep(.2);
}