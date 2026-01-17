const origin = typeof window !== 'undefined' && window.location.origin
  ? window.location.origin
  : 'https://twinsfashion.bg';

export const environment = {
  production: true,
  apiBaseUrl: origin
};

