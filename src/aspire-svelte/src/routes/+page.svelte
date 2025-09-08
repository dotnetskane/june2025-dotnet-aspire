<script lang="ts">
  import { onMount } from "svelte";
  import type { PageProps } from "./$types";

  interface TodoItem {
    id: number;
    title: string;
    description: string;
    isComplete: boolean;
    createdAt: Date;
  }

  type TodoList = TodoItem[];

  let { data }: PageProps = $props();
  let todos: TodoList = $state(data.todos);

  let selectedTodo = $state<TodoItem | null>(null);

  function selectTodo(todo: TodoItem) {
    selectedTodo = todo;
  }

  export function stopPropagation(node: HTMLElement, handler: () => void) {
    const onClick = (event: MouseEvent) => {
      event.stopPropagation();
      handler();
    };

    node.addEventListener("click", onClick);

    return {
      destroy() {
        node.removeEventListener("click", onClick);
      },
    };
  }

  // onMount(async () => {
  //   const response = await fetch("api/todos");
  //   todos = await response.json();
  // });

  async function deleteTodo(id: number) {
    const response = await fetch(`api/todos/${id}`, {
      method: "DELETE",
    });
    if (response.ok) {
      todos = todos.filter((todo) => todo.id !== id);
    } else {
      console.error("Failed to delete todo");
    }
  }

  async function addTodo(title: string, description: string) {
    const response = await fetch("api/todos", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ title, description }),
    });
    if (response.ok) {
      const newTodo = await response.json();
      todos = [...todos, newTodo];
    } else {
      console.error("Failed to add todo");
    }
  }
</script>

<div class="p-8">
  <h1 class="text-4xl font-semibold mb-8">Todo List (Svelte)</h1>

  <div class="">
    <div
      class="flex flex-col space-y-4 w-1/2 mt-10 p-4 border border-gray-300 rounded-lg shadow-sm"
    >
      <input
        id="newTodoInput"
        type="text"
        placeholder="Add a new todo"
        class="border border-gray-300 rounded-lg p-2"
      />
      <textarea
        rows="2"
        placeholder="Description"
        class="border border-gray-300 rounded px-4 py-2 resize-none focus:outline-none focus:ring-2 focus:ring-blue-500"
      ></textarea>

      <button
        class="self-start bg-indigo-600 text-white rounded-lg px-4 py-2"
        onclick={() => {
          const title = document.getElementById(
            "newTodoInput"
          ) as HTMLInputElement;
          if (!title || !title.value.trim()) return;
          const description = document.querySelector(
            "textarea"
          ) as HTMLTextAreaElement;
          if (!description || !description.value.trim()) return;

          addTodo(title.value.trim(), description.value.trim());
          title.value = "";
          description.value = "";
        }}
      >
        Add Todo
      </button>
    </div>

    <table
      class="table-auto min-w-full mt-10 divide-y divide-gray-200 border border-gray-300 rounded-lg overflow-hidden shadow-sm"
    >
      <thead
        class="font-bold p-4 border-b uppercase text-left bg-indigo-600 text-white"
      >
        <tr>
          <th class="px-4 py-2">Title</th>
          <th class="px-4 py-2">Description</th>
          <th class="px-4 py-2">Completed</th>
          <th class="px-4 py-2">Created At</th>
          <th class="px-4 py-2">
            <div class="flex items-center justify-between">
              Actions
              <button
                aria-label="Refresh"
                class="ml-3 hover:text-gray-300"
                onclick={async () => {
                  const response = await fetch("api/todos");
                  todos = await response.json();
                }}
              >
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  fill="none"
                  viewBox="0 0 24 24"
                  stroke-width="1.5"
                  stroke="currentColor"
                  class="size-6"
                >
                  <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    d="M16.023 9.348h4.992v-.001M2.985 19.644v-4.992m0 0h4.992m-4.993 0 3.181 3.183a8.25 8.25 0 0 0 13.803-3.7M4.031 9.865a8.25 8.25 0 0 1 13.803-3.7l3.181 3.182m0-4.991v4.99"
                  />
                </svg>
              </button>
            </div>
          </th>
        </tr>
      </thead>
      <tbody class="bg-white divide-y divide-gray-200">
        {#each todos as todo}
          <tr
            class="odd:bg-white even:bg-gray-50 hover:bg-gray-100"
            onclick={() => selectTodo(todo)}
          >
            <td class="px-4 py-2 whitespace-nowrap">{todo.title}</td>
            <td class="px-4 py-2 whitespace-nowrap">{todo.description}</td>
            <td class="px-4 py-2 whitespace-nowrap"
              >{todo.isComplete ? "Yes" : "No"}</td
            >
            <td class="px-4 py-2 whitespace-nowrap"
              >{new Date(todo.createdAt).toLocaleString()}</td
            >
            <td class="px-4 py-2 whitespace-nowrap">
              <button
                class="rounded bg-red-500 px-3 py-1 font-semibold text-white hover:text-red-900"
                use:stopPropagation={() => deleteTodo(todo.id)}
              >
                Delete
              </button>
            </td>
          </tr>
        {/each}
      </tbody>
    </table>

    {#if selectedTodo}
      <div class="mt-8 flex">
        <div
          class="w-full max-w-md bg-white border border-gray-300 rounded-lg shadow-lg p-6"
        >
          <div class="flex justify-between items-center mb-4">
            <h2 class="text-xl font-bold">{selectedTodo.title}</h2>
            <button
              class="text-gray-400 hover:text-gray-700"
              aria-label="Close"
              onclick={() => (selectedTodo = null)}
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                class="h-6 w-6"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M6 18L18 6M6 6l12 12"
                />
              </svg>
            </button>
          </div>
          <div class="mb-2 text-gray-600 text-sm">
            Created: {new Date(selectedTodo.createdAt).toLocaleString()}
          </div>
          <div class="text-gray-800 whitespace-pre-line">
            {selectedTodo.description}
          </div>
        </div>
      </div>
    {/if}
  </div>
</div>
