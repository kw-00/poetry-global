import React, { useEffect, useState } from "react";
import { useGlobalState } from "@/global-state/global-state";
import type{
  PersistedPoemMetadata,
  PersistedLanguage,
} from "../api/client";

export function PoemPanel({
  selectedPoem,
}: {
  selectedPoem: PersistedPoemMetadata | null;
}) {
  const state = useGlobalState();

  const [languages, setLanguages] = useState<PersistedLanguage[]>([]);
  const [languageId, setLanguageId] = useState<number | null>(null);

  const [loading, setLoading] = useState(false);

  const poem = state.getCurrentPoem();

  async function loadPoem(poemId: number, langId: number) {
    setLoading(true);
    try {
      const poem = await state.getPoemVersion(poemId, langId);

      if (!poem) {
        alert("Poem not found in selected language");
      }
    } catch {
      alert("Failed to load poem");
    } finally {
      setLoading(false);
    }
  }

  // load languages once
  useEffect(() => {
    async function load() {
      const langs = await state.getLanguages();
      setLanguages(langs);
      if (langs.length > 0) {
        setLanguageId(langs[0].id);
      }
    }

    load();
  }, []);

  // load poem when selection or language changes
  useEffect(() => {
    if (selectedPoem && languageId !== null) {
      loadPoem(selectedPoem.id, languageId);
    }
  }, [selectedPoem, languageId]);

  return (
    <div className="panel">
      <h2>Poem</h2>

      <div className="row">
        <label>Language:</label>
        <select
          className="input small"
          value={languageId ?? ""}
          onChange={(e) => setLanguageId(Number(e.target.value))}
        >
          {languages.map((l) => (
            <option key={l.id} value={l.id}>
              {l.code.toUpperCase()}
            </option>
          ))}
        </select>
      </div>

      {!selectedPoem && <div className="hint">Select a poem</div>}

      {loading && <div className="hint">Loading...</div>}

      {poem && (
        <div className="poemBox">
          <h3>{poem.title}</h3>
          <pre>{poem.versionText}</pre>
        </div>
      )}
    </div>
  );
}