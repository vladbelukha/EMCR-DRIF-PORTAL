import { defineConfig, Options } from 'orval';

const commonOutputConfig: Options['output'] = {
  mode: 'tags-split',
  client: 'angular',
  mock: false,
  prettier: true,  
  indexFiles: true,
  workspace: 'src',
  schemas: 'model',
  target: 'api',
};

const applicationApi: Options = {
  output: {
    ...commonOutputConfig,
  },
  input: {
    target: 'http://localhost:4020/api/openapi/v1/openapi.json',
  },
};

export default defineConfig({
  applicationApi
});
