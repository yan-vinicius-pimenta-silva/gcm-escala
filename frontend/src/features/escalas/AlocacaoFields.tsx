import { Autocomplete, TextField, Box, Typography } from '@mui/material';
import { useGuardas } from '../guardas/useGuardas';
import { useEquipes } from '../equipes/useEquipes';
import { useViaturas } from '../viaturas/useViaturas';
import { TipoSetor } from '../../types';
import type { Guarda, Equipe, Viatura } from '../../types';
import type { AlocacaoRequest } from '../../api/escalas';

interface Props {
  tipoSetor: TipoSetor;
  alocacoes: AlocacaoState;
  onChange: (state: AlocacaoState) => void;
}

export interface AlocacaoState {
  guardaIds: number[];
  equipeId: number | null;
  motoristaId: number | null;
  encarregadoId: number | null;
  apoioGuardaIds: number[];
  viaturaId: number | null;
}

export const emptyAlocacaoState: AlocacaoState = {
  guardaIds: [], equipeId: null, motoristaId: null,
  encarregadoId: null, apoioGuardaIds: [], viaturaId: null,
};

export function buildAlocacoes(tipoSetor: TipoSetor, state: AlocacaoState): AlocacaoRequest[] {
  const alocacoes: AlocacaoRequest[] = [];
  switch (tipoSetor) {
    case TipoSetor.Padrao:
      state.guardaIds.forEach(gid => alocacoes.push({ guardaId: gid, funcao: 'Integrante', viaturaId: state.viaturaId ?? undefined }));
      break;
    case TipoSetor.CentralComunicacoes:
      if (state.equipeId) alocacoes.push({ equipeId: state.equipeId, funcao: 'Integrante', viaturaId: state.viaturaId ?? undefined });
      break;
    case TipoSetor.RadioPatrulha:
      if (state.motoristaId) alocacoes.push({ guardaId: state.motoristaId, funcao: 'Motorista', viaturaId: state.viaturaId ?? undefined });
      if (state.encarregadoId) alocacoes.push({ guardaId: state.encarregadoId, funcao: 'Encarregado', viaturaId: state.viaturaId ?? undefined });
      state.apoioGuardaIds.forEach(gid => alocacoes.push({ guardaId: gid, funcao: 'Apoio', viaturaId: state.viaturaId ?? undefined }));
      break;
    case TipoSetor.DivisaoRural:
    case TipoSetor.RondaComercio:
      if (state.motoristaId) alocacoes.push({ guardaId: state.motoristaId, funcao: 'Motorista', viaturaId: state.viaturaId ?? undefined });
      if (state.encarregadoId) alocacoes.push({ guardaId: state.encarregadoId, funcao: 'Encarregado', viaturaId: state.viaturaId ?? undefined });
      break;
    case TipoSetor.Romu:
      if (state.motoristaId) alocacoes.push({ guardaId: state.motoristaId, funcao: 'Motorista', viaturaId: state.viaturaId ?? undefined });
      if (state.encarregadoId) alocacoes.push({ guardaId: state.encarregadoId, funcao: 'Encarregado', viaturaId: state.viaturaId ?? undefined });
      state.apoioGuardaIds.forEach(gid => alocacoes.push({ guardaId: gid, funcao: 'Integrante', viaturaId: state.viaturaId ?? undefined }));
      break;
  }
  return alocacoes;
}

export default function AlocacaoFields({ tipoSetor, alocacoes, onChange }: Props) {
  const { data: guardas = [] } = useGuardas();
  const { data: equipes = [] } = useEquipes();
  const { data: viaturas = [] } = useViaturas();
  const activeGuardas = guardas.filter((g: Guarda) => g.ativo);
  const activeEquipes = equipes.filter((e: Equipe) => e.ativo);
  const activeViaturas = viaturas.filter((v: Viatura) => v.ativo);

  const guardaSingle = (label: string, value: number | null, onSet: (id: number | null) => void) => (
    <Autocomplete
      sx={{ minWidth: 200 }}
      options={activeGuardas}
      getOptionLabel={(o: Guarda) => o.nome}
      value={activeGuardas.find(g => g.id === value) || null}
      onChange={(_, v) => onSet(v?.id ?? null)}
      renderInput={(params) => <TextField {...params} label={label} size="small" />}
    />
  );

  const guardaMulti = (label: string, value: number[], onSet: (ids: number[]) => void) => (
    <Autocomplete
      multiple sx={{ minWidth: 200 }}
      options={activeGuardas}
      getOptionLabel={(o: Guarda) => o.nome}
      value={activeGuardas.filter(g => value.includes(g.id))}
      onChange={(_, v) => onSet(v.map(g => g.id))}
      renderInput={(params) => <TextField {...params} label={label} size="small" />}
    />
  );

  return (
    <Box display="flex" flexDirection="column" gap={2}>
      <Typography variant="subtitle2">Alocacao</Typography>

      {tipoSetor === TipoSetor.Padrao && (
        guardaMulti('Guardas (Integrante)', alocacoes.guardaIds, (ids) => onChange({ ...alocacoes, guardaIds: ids }))
      )}

      {tipoSetor === TipoSetor.CentralComunicacoes && (
        <Autocomplete
          sx={{ minWidth: 200 }}
          options={activeEquipes}
          getOptionLabel={(o: Equipe) => o.nome}
          value={activeEquipes.find(e => e.id === alocacoes.equipeId) || null}
          onChange={(_, v) => onChange({ ...alocacoes, equipeId: v?.id ?? null })}
          renderInput={(params) => <TextField {...params} label="Equipe" size="small" />}
        />
      )}

      {(tipoSetor === TipoSetor.RadioPatrulha || tipoSetor === TipoSetor.DivisaoRural ||
        tipoSetor === TipoSetor.Romu || tipoSetor === TipoSetor.RondaComercio) && (
        <>
          {guardaSingle('Motorista', alocacoes.motoristaId, (id) => onChange({ ...alocacoes, motoristaId: id }))}
          {guardaSingle('Encarregado', alocacoes.encarregadoId, (id) => onChange({ ...alocacoes, encarregadoId: id }))}
        </>
      )}

      {tipoSetor === TipoSetor.RadioPatrulha && (
        guardaMulti('Apoio', alocacoes.apoioGuardaIds, (ids) => onChange({ ...alocacoes, apoioGuardaIds: ids }))
      )}

      {tipoSetor === TipoSetor.Romu && (
        guardaMulti('Integrante (opcional)', alocacoes.apoioGuardaIds, (ids) => onChange({ ...alocacoes, apoioGuardaIds: ids }))
      )}

      <Autocomplete
        sx={{ minWidth: 200 }}
        options={activeViaturas}
        getOptionLabel={(o: Viatura) => o.identificador}
        value={activeViaturas.find(v => v.id === alocacoes.viaturaId) || null}
        onChange={(_, v) => onChange({ ...alocacoes, viaturaId: v?.id ?? null })}
        renderInput={(params) => <TextField {...params} label="Viatura (opcional)" size="small" />}
      />
    </Box>
  );
}
