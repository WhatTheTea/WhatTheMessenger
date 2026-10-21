import exec from 'k6/execution';
import registerScenario from './register.js';
import { simulateChatUser, seedTestUsers } from './chatters.js';

const BASE_URL = __ENV.TARGET_URL || 'http://localhost:80';

export const options = {
  setupTimeout: 120000,
  scenarios: {
    register_flow: {
      executor: 'ramping-vus',
      startVUs: 0,
      stages: [
        { duration: '15s', target: 10 },
        { duration: '30s', target: 20 },
        { duration: '10s', target: 0 },
      ],
      exec: 'runRegister',
    },
    thousand_chatters: {
      executor: 'ramping-vus',
      startVUs: 0,
      stages: [
        { duration: '15s', target: 1000 },
        { duration: '30s', target: 1000 }
      ],
      exec: 'runChatters',
    },
  },
};

// Global setup runs ONCE before VUs start
export function setup() {
  // Check active scenarios selected for this run (e.g., via CLI --scenario)
  const activeScenarios = Object.keys(exec.test.options.scenarios);

  // Only seed database if the chatters scenario is running
  if (activeScenarios.includes('thousand_chatters')) {
    seedTestUsers(BASE_URL, 1000);
  } else {
    console.log('[SETUP] Skipping user seeding (thousand_chatters not active).');
  }
}

export function runRegister() {
  registerScenario(BASE_URL);
}

export function runChatters() {
  simulateChatUser(BASE_URL);
}