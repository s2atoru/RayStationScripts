def MarginDict(margins, type='Expand'):
    keys = ['Type', 'Superior', 'Inferior', 'Anterior', 'Posterior', 'Right', 'Left' ]
    values = [type] + margins

    return dict(zip(keys, values))