export function registerUnique() {
  const id = `${Date.now()}_${Math.random()}_${__VU}_${__ITER}`;
  return register(id)
}

export function register(id) {
    return {
    nickname: `user_${id}`,
    login: `user_${id}`,
    password: 'Password123!',
  };
}

export function login(id) {
  return {
    login: `user_${id}`,
    password: 'Password123!',
  };
}